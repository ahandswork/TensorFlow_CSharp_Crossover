//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using System.Web;
//using System.Net;

//namespace NeuralNetPractice
//{
//    class DataInterface
//    {
//        public static Databank GetDataCollection() => GetDataCollection(new string[] {
//                "AAPL",
//                "ADI",
//                "AMAT",
//                "CSCO",
//                "CSIOY",
//                "GOOG",
//                "INTC",
//                "LRCX",
//                "MSFT",
//                "STM",
//                "TXN",
//            },
//            DateTime.Now.AddDays(-100),
//            DateTime.Now
//            );
//        public static Databank GetDataCollection(string[] symbols, DateTime start, DateTime end)
//        {
//            Databank databank = new Databank();
//            foreach (var symbol in symbols)
//                databank.DataTableCollection.Add(GetData(symbol, start, end));
//            return databank;
//        }
//        public static DataTable GetData(string symbol, DateTime start, DateTime end)
//        {
//            DataTable building;
//            if (File.Exists("InputData\\" + symbol + ".csv"))
//            {
//                building = GetDataLocal(symbol, start, end);
//                if (building[0].StartingIndex == Program.SerializeDate(start))
//                    start = Program.DeserializeDate(building[0].EndingIndex + 1);

//                if (building[0].EndingIndex == Program.SerializeDate(end))
//                    end = Program.DeserializeDate(building[0].StartingIndex - 1);

//                if (end <= start)
//                    return building;
//            }
//            else
//                building = new DataTable();

//            return DataTable.Merge(building, GetDataRemote(symbol, start, end));
//        }
//        static DataTable GetDataLocal(string symbol, DateTime start, DateTime end) => DataTable.Load("InputData\\" + symbol + ".csv", "Date", (string str) => int.Parse(str), symbol).Select(Program.SerializeDate(start), Program.SerializeDate(end));
//        static DataTable GetDataRemote(string symbol, DateTime start, DateTime end)
//        {
//            //RemoteDataManager.
//            string json;
//            ServicePointManager.Expect100Continue = true;
//            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//            using (var wc = new System.Net.WebClient())
//                json = wc.DownloadString("https://api.worldtradingdata.com/api/v1/history?symbol=" + symbol + "&sort=newest&api_token=DqmZQ40MRT67vCkbMUTjUTzBqqrC1PiRMesLYOQP2jZU9tt3qSsIMwt6OaxV");

//            int startingIndex = Program.SerializeDate(start);
//            int length = Program.SerializeDate(end) - startingIndex + 1;
//            //table.Columns.AddRange(
//            //new DataColumn[]
//            //    {
//            //        new DataColumn("Close",startingIndex, new string[length]),
//            //    };
//            //);
//            int i = startingIndex;
//            for (DateTime t = start; t <= end; t = t.AddDays(1))
//            {
//                if (t.DayOfWeek == DayOfWeek.Saturday || t.DayOfWeek == DayOfWeek.Sunday)
//                    continue;
//                //table["Close"][i] = historical_prices[i].Price.ToString();
//                //table["High"][i] = stockQuote.Data[t].High.ToString();
//                //table["Low"][i] = stockQuote.Data[t].Low.ToString();
//                //table["Close"][i] = stockQuote.Data[t].Close.ToString();
//                //table["Adj Close"][i] = stockQuote.Data[t].AdjClose.ToString();
//                //table["Volume"][i] = stockQuote.Data[t].Volume.ToString();

//                i++;
//            }
//            //return table;
//            throw null;
//        }
//    }
//}
