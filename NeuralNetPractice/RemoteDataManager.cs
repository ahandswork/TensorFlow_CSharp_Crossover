//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Sheets.v4;
//using Google.Apis.Sheets.v4.Data;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//using System.Threading;
//using Newtonsoft.Json;
//using Google.Apis.Drive.v3;
//using Google.Apis.Download;

//namespace NeuralNetPractice
//{
//    class RemoteDataManager
//    {

//        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
//        static string ApplicationName = "Invester Data Collector";
//        public static void QueryData(DateTime startDate, DateTime endDate)
//        {
//            UserCredential credential;

//            using (var stream =
//                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
//            {
//                // The file token.json stores the user's access and refresh tokens, and is created
//                // automatically when the authorization flow completes for the first time.
//                string credPath = "token.json";
//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    Scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(credPath, true)).Result;
//                Console.WriteLine("Credential file saved to: " + credPath);
//            }

//            // Create Google Sheets API service.
//            var service = new SheetsService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = ApplicationName,
//            });

//            var stockData = GetStockData(startDate, endDate, "GOOG", service);
//        }
//        static DataColumn GetStockData(DateTime startDate, DateTime endDate, string company, SheetsService service)
//        {
//            //WriteToCell(
//            //    "A1",
//            //    "=GOOGLEFINANCE(\""
//            //    + company + "\", \"price\", DATE("
//            //    + startDate.Year + "," + startDate.Month
//            //    + "," + startDate.Day + "), DATE("
//            //    + endDate.Year + "," + endDate.Month
//            //    + "," + endDate.Day + "), \"DAILY\")",
//            //    service);
//            var cell = ReadCell("C2", service);
//            var column = ReadColumn('B', service);
//            column.Name = company + "." + column.Name;
//            column.StartingIndex = Program.SerializeDate(startDate);
//            return column;
//        }
//        static DataColumn ReadColumn(char column, SheetsService service)
//        {
//            string range = "Main!" + column + "1:" + column;
//            range = "Main!C2:C21";

//            SpreadsheetsResource.ValuesResource.GetRequest request =
//                    service.Spreadsheets.Values.Get(SPREADSHEET_ID, range);
//            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
//            ValueRange response = request.Execute();
//            List<string> parsed = new List<string>();
//            Console.WriteLine(JsonConvert.SerializeObject(response));
//            foreach (var list in response.Values)
//                foreach (var item in list)
//                    parsed.Add(item.ToString());
//            Console.WriteLine(JsonConvert.SerializeObject(response));
//            DataColumn dataColumn = new DataColumn(parsed[0].ToString(), 1, parsed.GetRange(1, parsed.Count - 1).ToArray());
//            return dataColumn;
//        }
//        static object ReadCell(string cellPos, SheetsService service)
//        {
//            SpreadsheetsResource.ValuesResource.
//            string range = "Main!" + cellPos;
//            SpreadsheetsResource.ValuesResource.GetRequest request =
//                    service.Spreadsheets.Values.Get(SPREADSHEET_ID, range);
//            request.

//            ValueRange response = request.Execute();
//            var result = JsonConvert.SerializeObject(response);
//            return result;
//        }
//        static bool WriteToCell(string pos, string value, SheetsService service)
//        {
//            ValueRange valueRange = new ValueRange();
//            valueRange.MajorDimension = "COLUMNS";

//            valueRange.Values = new List<IList<object>> { new List<object>() { value } };

//            var range = "Main!" + pos;
//            //range = "Main!A1";
//            valueRange.Range = range;
//            SpreadsheetsResource.ValuesResource.UpdateRequest request =
//                service.Spreadsheets.Values.Update(valueRange, SPREADSHEET_ID, range);
//            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

//            UpdateValuesResponse response = request.Execute();
//            return response.UpdatedCells == 1;
//        }
//    }
//}
