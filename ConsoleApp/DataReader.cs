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


        public void ImportData(string fileToImport)
        {
            ImportedObjects = new List<ImportedObject>();

            using (var streamReader = new StreamReader(fileToImport))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split(';');

                    if (values.Length >= 7)
                    {
                        var importedObject = new ImportedObject
                        {
                            Type = values[0].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(),
                            Name = values[1].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            Schema = values[2].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            ParentName = values[3].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            ParentType = values[4].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            DataType = values[5].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            IsNullable = values[6].Trim().Replace(" ", "").Replace(Environment.NewLine, "")
                        };

                        ((List<ImportedObject>)ImportedObjects).Add(importedObject);
                    }
                }
            }

     
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.NumberOfChildren = ImportedObjects.Count(impObj =>
                    impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name);
            }
        }

        public void PrintData()
        {
            foreach (var database in ImportedObjects.Where(obj => obj.Type == "DATABASE"))
            {
                int tableCount = ImportedObjects.Count(obj =>
                    obj.ParentType.ToUpper() == database.Type && obj.ParentName == database.Name);
                Console.WriteLine($"Database '{database.Name}' ({tableCount} tables)");

                foreach (var table in ImportedObjects.Where(obj =>
                    obj.ParentType.ToUpper() == database.Type && obj.ParentName == database.Name))
                {
                    int columnCount = ImportedObjects.Count(obj =>
                        obj.ParentType.ToUpper() == table.Type && obj.ParentName == table.Name);
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({columnCount} columns)");

                    foreach (var column in ImportedObjects.Where(obj =>
                        obj.ParentType.ToUpper() == table.Type && obj.ParentName == table.Name))
                    {
                        Console.WriteLine(
                            $"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }
            Console.ReadKey();
        }
        
    }







}

    class ImportedObject : ImportedObjectBaseClass
    {
  

        public ImportedObject () 
        {
            Type = "";
            Name = "";
            Schema = "";
            ParentName = "";
            ParentType = "";

        }
        public string Schema {  get; set; } 

        public string ParentName { get; set; }  
        public string ParentType { get; set; }
       
        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public int NumberOfChildren { get; set; } 
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

