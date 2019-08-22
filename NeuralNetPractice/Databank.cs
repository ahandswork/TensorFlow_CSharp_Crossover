using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeuralNetPractice
{
    class Databank
    {
        string Name { get; set; }
        public List<DataTable> DataTableCollection { get; } = new List<DataTable>();
        string GetFileName(string directory,bool isCSV = true)
        {
            while (directory.Contains('\\'))
                directory = directory.Substring(directory.IndexOf('\\') + 1);
            if(isCSV)
                directory = directory.Substring(0, directory.Length - 4);
            return directory;
        }
        //public static Databank LoadDatabankCSV(string filePath, string keyColumnName, Func<string, int> keyValueProcessor)
        //{
        //    Databank db = new Databank();

        //    List<string> files = Directory.GetFiles(filePath).ToList();
        //    for (int i = 0; i < files.Count; i++)
        //        if (files[i].Substring(files[i].Length - 4) != ".csv")
        //            files.RemoveAt(i--);
            
        //    foreach (var file in files)
        //        db.DataTableCollection.Add(DataTable.Load(file, keyColumnName, keyValueProcessor, db.GetFileName(file)));

        //    db.Name = db.GetFileName(filePath, false);
        //    return db;
        //}
        public DataTable ToDataTable()
        {
            DataTable combined = new DataTable();
            foreach (var table in DataTableCollection)
            {
                List<DataColumn> adding = new List<DataColumn>(table.ColumnCount);
                for(int i = 0; i < table.ColumnCount; i++)
                {
                    adding.Add(table.Columns[i].Copy());
                    adding.Last().Name = table.TableName + '.' + adding.Last().Name;
                }
                combined.Columns.AddRange(adding);
            }
            combined.TableName = Name;
            return combined;
        }
    }
}
