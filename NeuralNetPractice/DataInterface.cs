using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;

namespace NeuralNetPractice
{
    class DataInterface
    {
        const string PREPROCESS_STORAGE_DIRECTORY = "PreProcessData\\";
        public static Databank GetDataCollection() => GetDataCollection(new string[] {
                "AAPL",
                "ADI",
                "AMAT",
                "CSCO",
                "CSIOY",
                "GOOG",
                "INTC",
                "LRCX",
                "MSFT",
                "STM",
                "TXN",
            });
        public static Databank GetDataCollection(string[] symbols)
        {
            Databank databank = new Databank();
            foreach (var symbol in symbols)
                databank.DataTableCollection.Add(GetDataRemote(symbol));
            return databank;
        }
        class IncomingDataStructure
        {
            public string name;
            public Dictionary<object,object> history;
        }
        static string apiToken;
        static DataTable GetDataRemote(string symbol)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string json;
            if (File.Exists(PREPROCESS_STORAGE_DIRECTORY + symbol + ".txt") && File.GetLastWriteTime(PREPROCESS_STORAGE_DIRECTORY + symbol + ".txt").DayOfYear == DateTime.Now.DayOfYear)
                json = File.ReadAllText(PREPROCESS_STORAGE_DIRECTORY + symbol + ".txt");
            else
            {
                if (apiToken == null)
                {
                    Console.WriteLine("Please enter your world trading data api token");
                    apiToken = Console.ReadLine();
                }
                using (var wc = new WebClient())
                    json = wc.DownloadString("https://api.worldtradingdata.com/api/v1/history?symbol=" + symbol + "&sort=newest&api_token=" + apiToken);
                File.WriteAllText(PREPROCESS_STORAGE_DIRECTORY + symbol + ".txt", json);
            }
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingDataStructure>(json);
            //table.Columns.AddRange(
            //new DataColumn[]
            //    {
            //        new DataColumn("Close",startingIndex, new string[length]),
            //    };
            //);
            DataTable table = new DataTable();
            var lines = data.history.Values.ToArray();
            object[] strLines = new object[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                strLines[i] = lines[lines.Length - i - 1];
            lines = strLines;
            var dates = data.history.Keys.ToArray();
            strLines = new object[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                strLines[i] = dates[lines.Length - i - 1];
            dates = strLines;
            var firstLine = lines[0].ToString();
            DateTime startDate = Program.SerializeDate(data.history.Keys.First().ToString());
            table.Columns.AddRange( new DataColumn[]
                {
                    new DataColumn("Open"),
                    new DataColumn("Close"),
                    new DataColumn("High"),
                    new DataColumn("Low"),
                    new DataColumn("Volume"),
                    new DataColumn("Date")
                });
            //lines.Reverse();
            for (int i = 0; i < data.history.Count; i++)
            {
                var line = lines[i].ToString();
                int start = line.IndexOf("open\": \"") + 8;
                int length = line.IndexOf("\",") - start;

                var rawDate = dates[i].ToString();
                var key = Program.SerializeDate(rawDate);
                table["Date"][key] = rawDate;

                table["Open"][key] = line.Substring(start, length);

                start = line.IndexOf("close\": \"",start + length) + 9;
                length = line.IndexOf("\",",start) - start;
                table["Close"][key] = line.Substring(start, length);

                start = line.IndexOf("high\": \"", start + length) + 8;
                length = length = line.IndexOf("\",", start) - start;
                table["High"][key] = line.Substring(start, length);

                start = line.IndexOf("low\": \"", start + length) + 7;
                length = length = line.IndexOf("\",", start) - start;
                table["Low"][key] = line.Substring(start, length);

                start = line.IndexOf("volume\": \"", start + length) + 10;
                length = length = line.IndexOf("\"", start) - start;
                table["Volume"][key] = line.Substring(start, length);

                start = line.IndexOf("volume\": \"", start + length) + 10;
                length = length = line.IndexOf("\"", start) - start;
                table["Volume"][key] = line.Substring(start, length);
            }
            table.TableName = symbol;
            return table;
        }
    }
}
