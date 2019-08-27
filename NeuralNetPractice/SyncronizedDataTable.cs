using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetPractice
{
    class SyncronizedDataTable
    {
        private DateTime[] RowDates { get; set; }
        private string[] ColumnNames { get; set; }
        private string[,] Data { get; set; }//[Columns,rows]

        private int RowCount => Data.GetLength(1);
        private int ColumnCount => Data.GetLength(0);
        public DateTime FirstDay => RowDates[0];
        public DateTime LastDay => RowDates.Last();
        public string this[string columnName, DateTime rowDate]
        {
            get
            {
                return Data[GetColumnIndex(columnName), GetRowIndex(rowDate)];
            }
            set
            {
                Data[GetColumnIndex(columnName), GetRowIndex(rowDate)] = value;
            }
        }
        public DataColumn this[string columnName]
        {
            get
            {
                int columnIndex = GetColumnIndex(columnName);
                Dictionary<DateTime, string> content = new Dictionary<DateTime, string>();
                for (int i = 0; i < RowCount; i++)
                    content.Add(RowDates[i], Data[columnIndex, i]);
                return new DataColumn(columnName, content);
            }
            //set
            //{
            //    Data[GetColumnIndex(columnName), GetRowIndex(rowDate)] = value;
            //}
        }
        public SyncronizedDataTable(string[] columnNames, Dictionary<DateTime, List<string>> data)
        {
            ColumnNames = columnNames;
            Data = new string[data.First().Value.Count, data.Count];
            RowDates = new DateTime[RowCount];
            var rawDataArray = data.ToArray();
            for (int i = 0; i < RowCount; i++)
            {
                RowDates[i] = rawDataArray[i].Key;
                for (int j = 0; j < ColumnCount; j++)
                {
                    Data[j, i] = rawDataArray[i].Value[j];
                }
            }
        }
        public SyncronizedDataTable(DateTime[] rowDates, string[] columnNames, string[,] data)
        {
            RowDates = rowDates;
            ColumnNames = columnNames;
            Data = data;
        }
        private int GetRowIndex(DateTime date)
        {
            for (int row = 0; row < RowDates.Length; row++)
                if (RowDates[row] == date)
                    return row;
            return -1;
        }
        private int GetColumnIndex(string name)
        {
            for (int column = 0; column < ColumnNames.Length; column++)
                if (ColumnNames[column] == name)
                    return column;
            return -1;
        }
        public void Splice(out SyncronizedDataTable a, out SyncronizedDataTable b) =>
            Splice(out a, out b, 0.5);
        public void Splice(out SyncronizedDataTable a, out SyncronizedDataTable b, double abCountRatio) =>
            Splice(out a, out b, abCountRatio, new Random());
        public void Splice(out SyncronizedDataTable a, out SyncronizedDataTable b, double abCountRatio, Random ran)
        {
            string[,] dataB = new string[ColumnCount, (int)Math.Round(RowCount / (1 + abCountRatio))];
            string[,] dataA = new string[ColumnCount, RowCount - dataB.GetLength(1)];

            List<DateTime> aKeys = RowDates.ToList();
            List<DateTime> bKeys = new List<DateTime>();

            for (int i = 0; i < dataB.GetLength(1); i++)
            {
                int index = ran.Next(0, aKeys.Count);
                bKeys.Add(aKeys[index]);
                aKeys.RemoveAt(index);
            }
            bKeys.Sort();
            
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < aKeys.Count; j++)
                    dataA[i,j] = this[ColumnNames[i],aKeys[j]];
                for (int j = 0; j < bKeys.Count; j++)
                    dataB[i, j] = this[ColumnNames[i], bKeys[j]];
            }
            a = new SyncronizedDataTable(aKeys.ToArray(), ColumnNames, dataA);
            b = new SyncronizedDataTable(bKeys.ToArray(), ColumnNames, dataB);
        }
        public void RemoveColumn(string columnName) => RemoveColumn(GetColumnIndex(columnName));
        public void RemoveColumn(int columnIndex)
        {
            var newData = new string[ColumnCount - 1, RowCount];
            sbyte k = 0;
            for(int i = 0; i < ColumnCount; i++)
            {
                if(i == columnIndex)
                {
                    k = -1;
                    continue;
                }
                for (int j = 0; j < RowCount; j++)
                {
                    newData[i, j + k] = Data[i,j];
                }
            }
            Data = newData;
        }
        public SyncronizedDataTable Select(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate || startDate > RowDates.Last())
                throw new IndexOutOfRangeException("The requested date span is either negative or does not include any of the elements in this set.");
            int start = 0;
            for(; start < RowCount; start++)
                if(RowDates[start] >= startDate)
                    break;
            int end = start;
            for (; end < RowCount; end++)
                if (RowDates[start] <= endDate)
                    break;

            string[,] data = new string[ColumnCount, end - start + 1];
            List<DateTime> dateTimes = new List<DateTime>(data.GetLength(1));
            for (int i = start; i <= end; i++)
            {
                dateTimes.Add(RowDates[i]);
                for (int j = 0; j < ColumnCount; j++)
                    data[j, i - start] = Data[j, i];
            }
            return new SyncronizedDataTable(dateTimes.ToArray(), ColumnNames, data);
        }
        public static void Save(string path, SyncronizedDataTable set) => set.Save(path);
        public void Save(string path)
        {
            string[] lines = new string[RowCount + 1];
            for (int i = 0; i < lines.Length; i++)
                lines[i] = "";
            for (int i = 0; i < ColumnCount; i++)
            {
                lines[0] += ',' + ColumnNames[i];
                for (int j = 1; j < lines.Length; j++)
                {
                    lines[j] += ',' + Data[i,j - 1];
                }
            }
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Substring(1);
            File.WriteAllLines(path, lines);
            Console.WriteLine("Saving {0}, total lines: {1}", path, lines.Length);
        }
    }
}
