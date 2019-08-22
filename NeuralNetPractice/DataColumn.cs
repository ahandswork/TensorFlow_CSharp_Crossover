using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetPractice
{
    class DataColumn
    {
        DataColumn()
            :this("Untitled")
        { }
        public DataColumn(string name)
            :this(name,new Dictionary<DateTime, string>())
        { }
        public DataColumn(string name, Dictionary<DateTime,string> content)
        {
            Name = name;
            Content = content;
        }
        public string Name { get; set; }
        public int Length => Content.Count;
        public DateTime FirstDay => Content.Keys.First();
        public DateTime LastDay => Content.Keys.Last();
        Dictionary<DateTime,string> Content { get; set; }
        public string this[int i]
        {
            get
            {
                return Content.Values.ElementAt(i);
            }
            set
            {
                Content.Values.ToArray()[i] = value;
            }
        }
        public string this[DateTime time]
        {
            get
            {
                return Content[time];
            }
            set
            {
                if (value == null)
                    Content.Remove(time);
                else if (Content.ContainsKey(time))
                    Content[time] = value;
                else
                    Content.Add(time, value);
            }
        }
        public DateTime[] Keys => Content.Keys.ToArray();
        public DataColumn Select(DateTime startDate, DateTime endDate)
        {
            DataColumn dc = new DataColumn();
            dc.Name = Name;
            var last = Content.Last();
            foreach (var pair in Content)
                if(startDate <= pair.Key && pair.Key <= endDate)
                    dc.Content.Add(pair.Key, pair.Value);
            return dc;
        }
        //add
        public void Add(DateTime key, string value) => Content.Add(key,value);
        //there cannot be gaps between "a" and "b"
        public static DataColumn Merge(DataColumn a, DataColumn b) =>
            Merge(a, b, a.Name == b.Name ? a.Name : ("Merge(" + a.Name + ", " + b.Name + ")"));
        //there cannot be gaps between "a" and "b"
        public static DataColumn Merge(DataColumn a, DataColumn b, string columnName)
        {
            if (b.FirstDay < a.FirstDay)
                return Merge(b, a);
            if (b.LastDay < a.LastDay)
                return a.Copy();

            DataColumn c = new DataColumn(columnName, new Dictionary<DateTime,string>((int)(b.LastDay - a.FirstDay).TotalDays));

            DateTime i = a.Content.Keys.First();
            for (; i < a.Content.Keys.Last(); i = i.AddDays(1))
                c[i] = a[i];
            for (; i < b.Content.Keys.Last(); i = i.AddDays(1))
                c[i] = b[i];
            return c;
        }
        //augment
        public void NormalizeAtan()
        {
            foreach (var day in Keys)
                Content[day] = (2 / Math.PI * Math.Atan(double.Parse(Content[day]))).ToString();
        }
        public void NormalizeSigmoid()
        {
            foreach (var day in Keys)
                Content[day] = (1 / (1 + Math.Exp(-double.Parse(Content[day])))).ToString();
        }
        public void Augment(Func<string, string> f)
        {
            foreach (var day in Keys)
                Content[day] = f(Content[day]);
        }
        public DataColumn Copy()
        {
            var dictionary = new Dictionary<DateTime,string>(Content.Count);
            foreach (var pair in Content)
                dictionary.Add(pair.Key.AddDays(0), pair.Value);
            return new DataColumn(Name, dictionary);
        }
        public DataColumn ShiftDays(int days, bool skipWeekends)
        {
            DataColumn dataColumn = new DataColumn(Name, new Dictionary<DateTime, string>());
            if (skipWeekends)
            {
                foreach (var value in Content)
                {
                    var newDay = value.Key.AddDays(days);
                    if (newDay.DayOfWeek == DayOfWeek.Saturday || newDay.DayOfWeek == DayOfWeek.Sunday)
                        newDay.AddDays(2);
                    dataColumn.Content.Add(newDay, value.Value);
                }
            }
            else {

                foreach (var value in Content)
                    dataColumn.Content.Add(value.Key.AddDays(days), value.Value);
            }
            return dataColumn;
        }
        //extrapolate
        public DataColumn BuildDifferentialLabel(Func<string, string, string> f, int recordSeperation)
        {
            recordSeperation *= -1;
            DataColumn d = new DataColumn();
            d.Content = new Dictionary<DateTime, string>(Length - recordSeperation);
            d.Name = "Delta-" + Name;
            foreach (var day in Keys)
            {
                if (Content.Keys.Contains(day))
                {
                    var nextDay = day.AddDays(recordSeperation);
                    if (Content.Keys.Contains(nextDay))
                        d[day] = f(this[day], this[nextDay]);
                }
            }
            return d;
        }
        //add

        //remove
        public void Remove(DateTime key) => Content.Remove(key);
    }
}
