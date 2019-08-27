using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetPractice
{
    class DataTable : IEnumerable<DataColumn>
    {
        public string TableName { get; set; }
        public string KeyColumnName { get; set; }
        /// <summary>
        /// [Column,Row]
        /// </summary>
        public List<DataColumn> Columns { get; }
        public int ColumnCount => Columns.Count;
        public DataColumn this[string column]
        {
            get
            {
                return Columns[GetColumnIndex(column)];
            }
            set
            {
                Columns[GetColumnIndex(column)] = value;
            }
        }
        public DataColumn this[int column]
        {
            get
            {
                return Columns[column];
            }
            set
            {
                Columns[column] = value;
            }
        }
        public DataTable()
        {
            Columns = new List<DataColumn>();
        }
        //public static DataTable Load(string path, string tableName, string keyColumnName)
        //{
        //    string[] lines = File.ReadAllLines(path);
        //    var names = lines[0].Split(',').ToList();
        //    List<Dictionary<DateTime, string>> data = new List<Dictionary<DateTime, string>>();
        //    int keyColumnIndex = -1;
        //    for(int i = 0; i < names.Count; i++)
        //    {
        //        if (keyColumnIndex == -1 && names[i] == keyColumnName)
        //        {
        //            keyColumnIndex = i;
        //        }
        //        else
        //            data.Add(new Dictionary<DateTime, string>());
        //    }
        //    names.Remove(keyColumnName);
        //    if (keyColumnIndex == -1)
        //        throw new Exception("Key column not found.");
        //    for (int i = 1; i < lines.Length; i++)
        //    {
        //        //changes to 1 when the key column has been reached/found
        //        byte found = 0;
        //        var line = lines[i].Split(',');
        //        for (int j = 0; j < line.Length; j++)
        //        {
        //            //to skip the key column
        //            if (j == keyColumnIndex)
        //            {
        //                found = 1;
        //                continue;
        //            }
        //            data[j - found][i - 1] = line[j];
        //        }
        //    }
        //    DataTable dt = new DataTable();
        //    dt.TableName = tableName;
        //    dt.KeyColumnName = keyColumnName;
        //    for (int i = 0; i < data.Count; i++)
        //        dt.Columns.Add(new DataColumn(names[i], data[i]));
        //    return dt;
        //}
        public static void Save(string path, DataTable set) => set.Save(path);
        public void Save(string path)
        {
            DebugVerifySynced();
            string[] lines = new string[Columns[0].Length + 1];
            for (int i = 0; i < ColumnCount; i++)
            {
                lines[0] += ',' + Columns[i].Name;
                for (int j = 1; j < lines.Length; j++)
                {
                    lines[j] += ',' + Columns[i][j - 1];
                }
            }
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Substring(1);
            File.WriteAllLines(path, lines);
            Console.WriteLine("Saving {0}, total lines: {1}", path, lines.Length);
        }
        public static DataTable Merge(DataTable a, DataTable b) => Merge(a, b, a.TableName == b.TableName ? a.TableName : "Merge(" + a.TableName + ", "+ b.TableName + ")");
        public static DataTable Merge(DataTable a, DataTable b, string mergedTableName)
        {
            List<DataColumn> unsorted = new List<DataColumn>();
            unsorted.AddRange(a.Columns);
            unsorted.AddRange(a.Columns);

            DataTable merged = new DataTable();
            merged.TableName = mergedTableName;

            for(int i = 0; i < unsorted.Count; i++)
                for(int j = i + 1; j < unsorted.Count; j++)
                    if(unsorted[i].Name == unsorted[j].Name)
                    {
                        merged.AddColumn(DataColumn.Merge(unsorted[i], unsorted[j]));
                        unsorted.RemoveAt(i--);
                        unsorted.RemoveAt(j);
                        break;
                    }
            for (int i = 0; i < unsorted.Count; i++)
                unsorted[i] = unsorted[i].Copy();
            merged.Columns.AddRange(unsorted);
            return merged;
        }
        public int GetColumnIndex(string name)
        {
            for (int i = 0; i < Columns.Count; i++)
                if (name == Columns[i].Name)
                    return i;
            return -1;
        }
        /// <summary>
        /// combines line so that one record will also contain "Factor" amount of previous records (including the one already there).
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public DataTable Interlace(int multiplier, bool skipWeekends)
        {
            DataTable dataTable = new DataTable();
            foreach (var column in Columns)
                for (int i = 0; i <= multiplier; i++) {
                    var dupe = column.ShiftDays(i, skipWeekends);
                    if(i != 0)
                        dupe.Name += i.ToString();
                    dataTable.Columns.Add(dupe);
                }
            return dataTable;
        }
        public void NormalizeAtan()
        {
            foreach (var column in Columns)
                column.NormalizeAtan();
        }
        public void NormalizeSigmoid()
        {
            foreach (var column in Columns)
                column.NormalizeSigmoid();
        }
        //set methods
        public void Augment(Func<string, string> f)
        {
            foreach (DataColumn column in Columns)
                column.Augment(f);
        }
        public DataTable Synchronize()
        {
            DataTable newTable = new DataTable();
            DateTime start = Columns[0].FirstDay;
            DateTime end = Columns[0].LastDay;
            for(int i = 1; i < ColumnCount; i++)
            {
                if (Columns[i].FirstDay > start)
                    start = Columns[i].FirstDay;
                if (Columns[i].LastDay < end) 
                    end = Columns[i].LastDay;
            }
            foreach (var column in Columns)
                newTable.AddColumn(new DataColumn(column.Name));
            DateTime startTime = DateTime.Now;
            short k = 0;
            foreach (var day in Columns[0].Keys)
            {
                if (k % 500 == 0)
                {
                    var now = (DateTime.Now - startTime).TotalSeconds;
                    int timeRemaining = (int)Math.Round(now * Columns[0].Length / k - now);//in seconds
                    Console.WriteLine("Syncronize: Estimated time remaining is {0} minutes and {1} seconds. ", timeRemaining / 60, timeRemaining);
                }
                bool rowComplete = true;
                for (int j = 0; j < ColumnCount; j++)
                    if (!Columns[j].Keys.Contains(day))
                    {
                        rowComplete = false;
                        break;
                    }
                if (rowComplete)
                    for (int i = 0; i < ColumnCount; i++)
                        newTable[i].Add(day,Columns[i][day]);
                k++;
            }
            newTable.DebugVerifySynced();
            return newTable;
        }
        void DebugVerifySynced()
        {
            if (Program.DEBUG)
            {
                foreach (var column in Columns)
                    if (column.Length != Columns[0].Length)
                        throw new Exception("Error DataTable not synced becuase column lengths are inconsistant.");
                foreach(var key in Columns[0].Keys)
                    for (int j = 1; j < Columns.Count; j++)
                        if (!Columns[j].Keys.Contains(key))
                            throw new Exception("Error mismatched column DateTimes.");
            }
        }
        public DataTable Select(DateTime startDate, DateTime endDate)
        {
            DataTable dataTable = new DataTable();
            foreach (var column in Columns)
                dataTable.AddColumn(column.Select(startDate, endDate));
            return dataTable;
        }
        //remove methods
        public void RemoveColumn(string columnName) => Columns.RemoveAt(GetColumnIndex(columnName));
        //add methods
        public void AddColumn(DataColumn column) => Columns.Add(column);

        public void AddColumn(Func<string, DateTime, string> generator, string sourceColumn, string name) =>
            AddColumn((string[] str, DateTime dateTime) => generator(str[0], dateTime), new string[] { sourceColumn }, name);

        public void AddColumn(Func<string, DateTime, string> generator, string sourceColumn, string name, DateTime startDate, DateTime endDate) =>
            AddColumn((string[] str, DateTime dateTime) => generator(str[0], dateTime), new string[] { sourceColumn }, name, startDate, endDate);

        public void AddColumn(Func<string[], DateTime, string> generator, string[] sourceColumns, string name) =>
            AddColumn(generator, sourceColumns, name, Columns[0].FirstDay, Columns[0].LastDay);

        public void AddColumn(Func<string[], DateTime, string> generator, string[] sourceColumnNames, string name, DateTime startDate, DateTime endDate)
        {
            //will give the values from the source columns in the order they are specified
            Dictionary<DateTime,string> content = new Dictionary<DateTime, string>();
            DataColumn[] sourceColumns = new DataColumn[sourceColumnNames.Length];
            for(int i = 0; i < sourceColumns.Length; i++)
                for(int j = 0; j < Columns.Count; j++)
                    if(Columns[j].Name == sourceColumnNames[i])
                    {
                        sourceColumns[i] = Columns[j];
                        break;
                    }
            string[] input = new string[sourceColumns.Length];
            for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
            {
                for (int j = 0; j < input.Length; j++)
                    input[j] = sourceColumns[j][i];
                content.Add(i, generator(input, i));
            }
            DataColumn column = new DataColumn(name, content);
            Columns.Add(column);
        }
        public void Splice(out DataTable a, out DataTable b, double abCountRatio) =>
            Splice(out a, out b, abCountRatio, new Random());
        public void Splice(out DataTable a, out DataTable b,double abCountRatio, Random ran)
        {
            int length = Columns[0].Length;
            int sizeB = (int)Math.Round(length/(1+abCountRatio));
            int sizeA = length - sizeB;

            List<DateTime> aKeys = this[0].Keys.ToList();
            List<DateTime> bKeys = new List<DateTime>();

            for (int i = 0; i < sizeB; i++)
            {
                int index = ran.Next(0, aKeys.Count);
                bKeys.Add(aKeys[index]);
                aKeys.RemoveAt(index);
            }
            bKeys.Sort();
            a = new DataTable();
            b = new DataTable();
            foreach(var column in Columns)
            {
                var aDictionary = new Dictionary<DateTime, string>();
                var bDictionary = new Dictionary<DateTime, string>();

                foreach (var key in aKeys)
                    aDictionary.Add(key, column[key]);
                foreach (var key in bKeys)
                    bDictionary.Add(key, column[key]);
                
                a.AddColumn(new DataColumn(column.Name, aDictionary));
                b.AddColumn(new DataColumn(column.Name, bDictionary));
            }
        }
        public IEnumerator<DataColumn> GetEnumerator()
        {
            return (IEnumerator<DataColumn>)Columns;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)Columns;
        }
    }
}
