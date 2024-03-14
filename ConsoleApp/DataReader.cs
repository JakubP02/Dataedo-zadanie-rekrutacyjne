namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;
       
          
        public void ImportAndPrintData(string fileToImport)           /* usunięto pole printData, ponieważ wartość nie jest nigdzie używana i
         jeśli metoda ma służyć jak nazwa wskazuje do pobierania i wyświetlania danych to definiowanie tego pola jest nieuzasadnione. 
         Jeśli w programie dane będą w jednym miejscu tylko pobierane, a w innym pobierane i wyświetlane to należy stworzyć dwie metody,
         jedną tylko do pobierania drugą tylko do wyświetlania. 
   */
        { 
            ImportedObjects = new List<ImportedObject>() { new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)                                                        
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 0; i < importedLines.Count; i++) /* usunięto znak <= i zastąpiono znakiem < aby wartość była mniejsza niż rozmiar kolekcji
                                                            i żeby nie generowało wyjątku System.ArgumentOutOfRangeException "Indeks był spoza zakresu" */
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();

                if (values.Length >= 7)  // dodano warunek if, aby indeks nie wykarczał po za granicę tablicy 
               {
                    importedObject.Type = values[0];
                    importedObject.Name = values[1];
                    importedObject.Schema = values[2];
                    importedObject.ParentName = values[3];
                    importedObject.ParentType = values[4];
                    importedObject.DataType = values[5];
                    importedObject.IsNullable = values[6];
                    ((List<ImportedObject>)ImportedObjects).Add(importedObject);
               }
            }

           
                // clear and correct imported data
                foreach (var importedObject in ImportedObjects)
                {
                    importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                    importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                    importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                }

   

            /*
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type)
                    {
                        if (impObj.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                        }
                    }
                }
            }



        foreach (var database in ImportedObjects)
        {
            if (database.Type == "DATABASE")
            {
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                // print all database's tables
                foreach (var table in ImportedObjects)
                {
                    if (table.ParentType.ToUpper() == database.Type)
                    {
                        if (table.ParentName == database.Name)
                        {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                            // print all table's columns
                            foreach (var column in ImportedObjects)
                            {
                                if (column.ParentType.ToUpper() == table.Type)
                                {
                                    if (column.ParentName == table.Name)
                                    {
                                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        */   // zakomentowano fragment kodu odpowiedzialny za zliczanie baz danych i tabel oraz ich wyświetlanie, ponieważ kod obliczał i podawał złe wyniki

            // poniżej przedstawiam propozycję kodu ograniczającą zagnieżdzanie pętli oraz warunków if przez co kod jest bardziej czytelniejszy


            // assign number of children
            foreach (var importedObject in ImportedObjects)
            {

                importedObject.NumberOfChildren = ImportedObjects.Count(impObj => impObj.ParentType ==
                importedObject.Type && impObj.ParentName == importedObject.Name);

            }
            foreach (var database in ImportedObjects.Where(obj => obj.Type == "DATABASE"))
            {
                int tableCount = ImportedObjects.Count(obj => obj.ParentType.ToUpper() == database.Type && obj.ParentName == database.Name);
                Console.WriteLine($"Database '{database.Name}' ({tableCount} tables)");


                foreach (var table in ImportedObjects.Where(obj => obj.ParentType.ToUpper() == database.Type && obj.ParentName == database.Name))
                {
                    int columnCount = ImportedObjects.Count(obj => obj.ParentType.ToUpper() == table.Type && obj.ParentName == table.Name);
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({columnCount} columns)");

             
                    foreach (var column in ImportedObjects.Where(obj => obj.ParentType.ToUpper() == table.Type && obj.ParentName == table.Name))
                    {
                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }





            Console.ReadLine();
            
            
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        /*public string Name
        {
            get;  // nie potrzebne pole, ta klasa dziedzicy po klasie  ImportedObjectBaseClass, gdzie to pole jest juz zdefiniowane
            set;
        }
        */

        public ImportedObject () /* stworzono konstruktor klasy, a w nim ustawiono wartości pól na pusty ciąg znaków, aby uniknąć przypisania wartości null,
                                  a w konsekwencji czego wystąpienia wyjątku System.NullReferenceException: */
        {
            Type = "";
            Name = "";
            Schema = "";
            ParentName = "";
            ParentType = "";

        }
        public string Schema {  get; set; } // dodano get i set 

        public string ParentName { get; set; } // dodano get i set 
        public string ParentType { get; set; }
       
        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public int NumberOfChildren { get; set; } // zmieniono na typ pola na int, gdyż to pole zawwsze będzie liczbą całkowitą oraz dodano get i set 
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
