//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeuralNetPractice
//{
//    class DataSet
//    {
//        public static DataSet Load(string path)
//        {
//            string[] lines = File.ReadAllLines(path);
//            string[] names = lines[0].Split(',');
//            string[,] data = new string[names.Length, lines.Length - 1];
//            for (int i = 1; i < lines.Length; i++)
//            {
//                var line = lines[i].Split(',');
//                for (int j = 0; j < line.Length; j++)
//                    data[j, i - 1] = line[j];
//            }
//            return new DataSet(data, names);
//        }
//        public static void Save(string path, DataSet set) => set.Save(path);
//        public void Save(string path)
//        {
//            string[] lines = new string[RecordCount + 1];
//            for (int i = 0; i < ColumnCount; i++)
//            {
//                lines[0] += ',' + ColumnNames[i];
//                for (int j = 1; j <= RecordCount; j++)
//                {
//                    lines[j] += ',' + Records[i, j - 1];
//                }
//            }
//            for (int i = 0; i < lines.Length; i++)
//                lines[i] = lines[i].Substring(1);
//            File.WriteAllLines(path, lines);
//            Console.WriteLine("Saving {0}, total lines: {1}", path, lines.Length);
//        }
//        DataSet(string[,] records, string[] headers)
//        {
//            Records = records;
//            ColumnNames = headers;
//        }
//        public string[] ColumnNames { get; private set; }
//        /// <summary>
//        /// [Column,Row]
//        /// </summary>
//        public string[,] Records { get; private set; }
//        public int ColumnCount => ColumnNames.Length;
//        public int RecordCount => Records.GetLength(1);
//        public string GetValue(int row, string column) => Records[GetColumnIndex(column), row];
//        public int GetColumnIndex(string name)
//        {
//            for (int i = 0; i < ColumnNames.Length; i++)
//                if (name == ColumnNames[i])
//                    return i;
//            return -1;
//        }
//        /// <summary>
//        /// combines line so that one record will also contain "Factor" amount of previous records (including the one already there).
//        /// </summary>
//        /// <param name="raw"></param>
//        /// <returns></returns>
//        public static DataSet Interlace(DataSet raw, int factor)
//        {
//            string[,] processed = new string[factor * raw.ColumnCount, raw.RecordCount - factor + 1];
//            string[] headers = new string[raw.ColumnCount * factor];
//            for (int i = 0; i < raw.ColumnCount; i++)//columns
//            {
//                for (int k = 0; k < factor; k++)
//                {
//                    headers[i * factor + k] = raw.ColumnNames[i] + k.ToString();
//                    for (int j = 0; j + k < raw.RecordCount && j < processed.GetLength(1); j++)//rows
//                    {
//                        processed[i * factor + k, j] = raw.Records[i, j + k];
//                    }
//                }
//            }
//            return new DataSet(processed, headers);
//        }
//        public void NormalizeAtan()
//        {
//            for (int i = 0; i < Records.GetLength(0); i++)
//                for (int j = 0; j < Records.GetLength(1); j++)
//                {
//                    if (double.TryParse(Records[i, j], out double n))
//                        Records[i, j] = (2 / Math.PI * Math.Atan(n)).ToString();
//                    else
//                        break;
//                }
//        }
//        public void NormalizeSigmoid()
//        {
//            for (int i = 0; i < Records.GetLength(0); i++)
//                for (int j = 0; j < Records.GetLength(1); j++)
//                {
//                    if (double.TryParse(Records[i, j], out double n))
//                        Records[i, j] = (1 / (1 + Math.Exp(-n))).ToString();
//                    else
//                        break;
//                }
//        }
//        public void NormalizeAtan(string column)
//        {
//            int i = GetColumnIndex(column);
//            for (int j = 0; j < Records.GetLength(1); j++)
//            {
//                double n = int.Parse(Records[i, j]);
//                Records[i, j] = (2 / Math.PI * Math.Atan(n)).ToString();
//            }
//        }
//        public DataSet BuildDifferentialLabel(Func<string, string, string> f, int recordSeperation, string columnName) =>
//            BuildDifferentialLabel(f, recordSeperation, GetColumnIndex(columnName));
//        public DataSet BuildDifferentialLabel(Func<string, string, string> f, int recordSeperation, int columnIndex)
//        {
//            DataSet d = new DataSet(new string[1, RecordCount - recordSeperation], new string[] { ColumnNames[columnIndex] + "(Differential)" });
//            for (int i = 0; i < RecordCount - recordSeperation; i++)
//            {
//                d.Records[0, i] = f(Records[columnIndex, i], Records[columnIndex, i + recordSeperation]);
//            }
//            string[,] newRecords = new string[ColumnCount, d.RecordCount];
//            for (int i = 0; i < ColumnCount; i++)
//                for (int j = 0; j < d.RecordCount; j++)
//                    newRecords[i, j] = Records[i, j];
//            Records = newRecords;
//            return d;
//        }
//        public void Splice(out DataSet a, out DataSet b) => Splice(out a, out b, new Random());
//        public void Splice(out DataSet a, out DataSet b, Random ran)
//        {
//            int newSizeA = RecordCount / 2;
//            int newSizeB = RecordCount / 2 + RecordCount % 2;
//            List<int> indexes = new List<int>(RecordCount);
//            for (int i = 0; i < RecordCount; i++)
//                indexes.Add(i);
//            a = new DataSet(new string[ColumnCount, newSizeA], new string[ColumnCount]);
//            b = new DataSet(new string[ColumnCount, newSizeB], new string[ColumnCount]);
//            for (int i = 0; i < ColumnCount; i++)
//                a.ColumnNames[i] = b.ColumnNames[i] = ColumnNames[i];
//            for (int i = 0; i < newSizeA; i++)
//            {
//                int randomNumber = ran.Next(0, indexes.Count);
//                int index = indexes[randomNumber];
//                indexes.RemoveAt(randomNumber);
//                for (int j = 0; j < ColumnCount; j++)
//                    a.Records[j, i] = Records[j, index];
//            }


//            for (int i = 0; i < newSizeB; i++)
//                for (int j = 0; j < ColumnCount; j++)
//                    b.Records[j, i] = Records[j, indexes[i]];
//        }
//        //get methods
//        public DataSet GetColumn(string columnName) => GetColumn(columnName, GetColumnIndex(columnName));
//        public DataSet GetColumn(int columnIndex) => GetColumn(ColumnNames[columnIndex], columnIndex);
//        public DataSet GetColumn(string columnName, int columnIndex)
//        {
//            DataSet d = new DataSet(new string[1, RecordCount], new string[] { columnName });
//            for (int i = 0; i < RecordCount; i++)
//                d.Records[0, i] = Records[columnIndex, i];
//            return d;
//        }
//        //set methods
//        public void AugmentColumn(Func<string, string> f, string columnName) =>
//            AugmentColumn(f, GetColumnIndex(columnName));
//        public void AugmentColumn(Func<string, string> f, int columnIndex)
//        {
//            for (int i = 0; i < RecordCount; i++)
//                Records[columnIndex, i] = f(Records[columnIndex, i]);
//        }
//        //remove methods
//        public void RemoveColumn(string name)
//        {
//            string[] newHeaders = new string[ColumnCount - 1];
//            string[,] newData = new string[newHeaders.Length, RecordCount];
//            byte found = 0;
//            for (int i = 0; i < newData.GetLength(0); i++)
//            {
//                if (ColumnNames[i] == name && i < newHeaders.Length)
//                    found = 1;
//                for (int j = 0; j < RecordCount; j++)
//                    newData[i, j] = Records[i + found, j];
//                newHeaders[i] = ColumnNames[i + found];
//            }
//            Records = newData;
//            ColumnNames = newHeaders;
//        }
//        public void RemoveRecord(int index) => RemoveRecords(index, 1);
//        public void RemoveRecords(int index, int count)
//        {
//            string[,] newData = new string[ColumnCount, RecordCount - count];
//            int j = 0;
//            for (; j < index; j++)
//            {
//                for (int i = 0; i < ColumnCount; i++)
//                    newData[i, j] = Records[i, j];
//            }
//            j += count;
//            for (; j < RecordCount; j++)
//            {
//                for (int i = 0; i < ColumnCount; i++)
//                    newData[i, j - count] = Records[i, j];
//            }
//            Records = newData;
//        }
//        //add methods
//        public void AddColumn(DataSet column)
//        {
//            int recordCount = Math.Min(column.RecordCount, RecordCount);
//            string[] newHeaders = new string[ColumnCount + 1];

//            string[,] newData = new string[newHeaders.Length, recordCount];
//            for (int j = 0; j < recordCount; j++)
//                newData[0, j] = column.Records[0, j];
//            for (int i = 0; i < ColumnCount; i++)
//            {
//                for (int j = 0; j < recordCount; j++)
//                    newData[i + 1, j] = Records[i, j];
//                newHeaders[i + 1] = ColumnNames[i];
//            }
//            Records = newData;
//            ColumnNames = newHeaders;
//        }
//    }
//}
