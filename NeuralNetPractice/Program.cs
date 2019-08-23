using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetPractice
{
    class Program
    {
        public const bool DEBUG = true;
        public static DateTime SerializeDate(string rawDate)
        {
            var elements = rawDate.Split('-');
            return new DateTime(int.Parse(elements[0]), int.Parse(elements[1]), int.Parse(elements[2]));
        }
        static long DeterminantSolver(long[,] matrix)
        {
            if (matrix.GetLength(0) == 2 && matrix.GetLength(1) == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            long solution = 0;
            long[,] smallerMatrix = new long[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < i; j++)
                    for (int h = 0; h < smallerMatrix.GetLength(1); h++)
                        smallerMatrix[j, h] = matrix[j, h + 1];

                for (int j = i; j < smallerMatrix.GetLength(0); j++)
                    for (int h = 0; h < smallerMatrix.GetLength(1); h++)
                        smallerMatrix[j, h] = matrix[j + 1, h + 1];

                solution += (i % 2 == 0 ? 1 : -1) * DeterminantSolver(smallerMatrix) * matrix[i, 0];
            }
            return solution;
        }
        static Fraction[] SystemsOfEquationsSolver(long[,] lhs, long[] rhs)
        {
            long D = DeterminantSolver(lhs);
            long[,] matrix = new long[lhs.GetLength(0), lhs.GetLength(1)];
            Fraction[] solution = new Fraction[lhs.GetLength(0)];
            for (int i = 0; i < solution.Length; i++)
            {
                long[] cp = new long[lhs.GetLength(1)];
                for (int h = 0; h < matrix.GetLength(1); h++)
                {
                    cp[h] = lhs[i, h];
                    lhs[i, h] = rhs[h];
                }
                solution[i] = new Fraction(DeterminantSolver(lhs), D);
                solution[i].Simplify();
                for (int h = 0; h < matrix.GetLength(1); h++)
                    lhs[i, h] = cp[h];
            }
            return solution;
        }
        public static Polynomial GeneratePolynomial(long[] x, long[] y)
        {
            long[,] rhs = new long[x.Length, x.Length];
            for(int i = 0; i < x.Length; i++)
            {
                long z = 1;
                for(int j = 0;j<x.Length; j++)
                {
                    rhs[j, i] = z;
                    z *= x[i];
                }
            }
            return new Polynomial(SystemsOfEquationsSolver(rhs, y));
        }
        static bool OutDated(string directory) =>
            !System.IO.File.Exists(directory) || (DateTime.Now - System.IO.File.GetLastWriteTime(directory)).TotalDays >= 1;
        static void Main(string[] args)
        {
            const string POST_PROCESS_SAVE_LOCATION = "PostProcessData\\";

            if (OutDated(POST_PROCESS_SAVE_LOCATION + "testingLabelSet.csv"))
            {
                Console.WriteLine("Starting Import");
                var databank = DataInterface.GetDataCollection();
                Console.WriteLine("Import Complete");
                Console.WriteLine("Preprocessing");
                //Console.ReadLine();
                //Databank databank = Databank.LoadDatabankCSV("InputData","Date",(string rawDate) =>
                //{
                //    var elements = rawDate.Split('-');
                //    DateTime date = new DateTime(int.Parse(elements[0]), int.Parse(elements[1]), int.Parse(elements[2]));
                //    return (int)(date.DayOfYear + (date.Year - 1950) * 260);
                //});
                var dateColumn = databank.DataTableCollection[0]["Date"];
                foreach (var t in databank.DataTableCollection)
                {
                    //t.RemoveColumn("High");
                    //t.RemoveColumn("Low");
                    t.RemoveColumn("Volume");
                    t.RemoveColumn("Date");
                }
                DataTable table = databank.ToDataTable();
                for (int i = 0; i < table.ColumnCount; i++)
                    Console.WriteLine("Name: {0}, Starting Index: {1}, Ending Index: {2}", table.Columns[i].Name, table.Columns[i].FirstDay, table.Columns[i].LastDay);
                //table = table.Interlace(2,true);
                //table.AddColumn(dateColumn);
                //Console.WriteLine("LastDate: " + table["Date"][table["Date"].Length - 1]);
                table = table.Synchronize();
                table.AddColumn((string[] strInput, DateTime dateTime) => dateTime.DayOfYear.ToString(), new string[0], "DayOfYear");
                var label = table["TXN.Close"].BuildDifferentialLabel((string current, string future) =>
                {
                    return future;
                //return ((Math.Sign(double.Parse(future) - double.Parse(current)) + 1) / 2).ToString();
                }, 1);
                label.Name = "label";
                table.AddColumn(label);
                table = table.Synchronize();

                label = table["label"];
                table.RemoveColumn("label");
                const int RANDOM_SEED = 165113;
                table = table.Select(label.FirstDay, label.LastDay);
                var latestDate = table[0].LastDay;
                table.Select(latestDate, latestDate).Save(POST_PROCESS_SAVE_LOCATION + "latest.csv");

                table.Splice(out DataTable trainingSet, out DataTable testingSet, 0.8, new Random(RANDOM_SEED));

                DataTable labelTable = new DataTable();
                labelTable.Columns.Add(label);
                //labelTable.AddColumn((string[] str, DateTime dt) => dt.ToString(), new string[0], "DateTime");

                labelTable.Splice(out DataTable trainingLabelSet, out DataTable testingLabelSet, 0.8, new Random(RANDOM_SEED));

                Console.WriteLine("Label Index: {0}, Ending Index: {1}", label.FirstDay, label.LastDay);

                testingSet.Save(POST_PROCESS_SAVE_LOCATION + "testingSet.csv");
                trainingSet.Save(POST_PROCESS_SAVE_LOCATION + "trainingSet.csv");
                testingLabelSet.Save(POST_PROCESS_SAVE_LOCATION + "testingLabelSet.csv");
                trainingLabelSet.Save(POST_PROCESS_SAVE_LOCATION + "trainingLabelSet.csv");
                Console.WriteLine("Preprocessing complete");
            }
            switch(ShowDialog("Which would you like to do?", new string[] { "Train network", "Test Network", "Predict using Network" }))
            {
                case 0:
                    Console.WriteLine("How many epochs would you like?");
                    Console.WriteLine(NeuralNetwork.Train(GetInteger(0,1000*1000*1000)));
                    break;
                case 1:
                    Console.WriteLine(NeuralNetwork.Test());
                    break;
                case 2:
                    Console.WriteLine(NeuralNetwork.PredictFuture());
                    break;
            }
            Console.WriteLine("Complete.");
            Console.ReadLine();
        }
        static int ShowDialog(string prompt, string[] options)
        {
            Console.WriteLine("\n" + prompt);
            for(int i = 0; i < options.Length; i++)
            {
                Console.WriteLine("    {0}: {1}", i, options[i]);
            }
            return GetInteger(0, options.Length);
        }
        static int GetInteger(int min, int max)
        {
            Console.WriteLine("Please Enter an integer in the following range: [{0},{1}).", min, max);
            if (int.TryParse(Console.ReadLine(), out int output))
            {
                if (min >= 0 && output < max)
                    return output;
                else
                {
                    Console.WriteLine("Error- value entered out of range.");
                    return GetInteger(min, max);
                }
            }
            else
            {
                Console.WriteLine("Error- value entered not an integer.");
                return GetInteger(min, max);
            }
        }
        /*
        static string GetString(double[] array)
        {
            string val = "{ ";
            for(int i = 0; i < array.Length-1; i++)
            {
                val += array[i] + ", ";
            }
            val += array[array.Length-1] + " }";
            return val;
        }
        static double[][] BaseFunction(double[][] input)
        {
            return new double[][]
            {
                new double[]
                {

                },
            };
        }*/
    }
}
