using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using WindowsMobile.annotation;

namespace WindowsMobile.Utils
{
    public class Tools
    {
        public static TimeSpan ToTime(string str)
        {
            string[] times = str.Split(':');
            int seconds = int.Parse(times[0]) * 60 + int.Parse(times[1]);

            return TimeSpan.FromSeconds(seconds);
        }

        public static string Format(decimal seconds)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(Double.Parse(seconds.ToString()));
            //double p = timespan.TotalSeconds;
            return timespan.Hours + ":" + timespan.Minutes + ":" + timespan.Seconds;
        }

        public static string Format(string date)
        {
            string[] d_h = date.Split(" ".ToCharArray());
            string[] dsplit = d_h[0].Split("/".ToCharArray());
            string d = dsplit[2] + "-" + dsplit[1] + "-" + dsplit[0] + " " + d_h[1];

            return d;
        }

        public static FieldInfo[] GetAllFields(Type t, string[] cols)
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            while(t != typeof(System.Object))
            {
                FieldInfo[] flds = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

                foreach(FieldInfo f in flds)
                {
                    if (Util.IsEmpty(cols) || cols.Contains(f.Name)) fields.Add(f);
                }

                t = t.BaseType;
            }

            return fields.ToArray<FieldInfo>();
        }

        public static bool checkIfId(FieldInfo f, Object instance) {
            return f.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase) && f.FieldType.Name.Equals("Int32") && (int)f.GetValue(instance) == 0;            
        }

        public static bool checkIfIdUpdate(FieldInfo f, Object instance, List<String> notAdd)
        {
            String[] tabNotAdd = notAdd.ToArray();
            for (int i = 0; i < tabNotAdd.Length; i++) {
                bool b =  f.Name.Equals(tabNotAdd[i], StringComparison.CurrentCultureIgnoreCase);
                if (b == true) {
                    return b;
                }
            }
            return false;
        }

        public static String firstUpperCase(String mot)
        {
            try
            {
                String final = "";
                String first = "";
                String ambiny = "";
                final = mot.ToLower();
                first = final.Substring(0, 1).ToUpper();
                ambiny = final.Substring(1, final.Length - 1);
                final = first + ambiny;
                return final;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                throw e;
            }
        } 

        public static void convertType(PropertyInfo property, Object obj, string value) {
            if (property.PropertyType.Name.ToLower().CompareTo("string") == 0)
            {
                property.SetValue(obj, value,null);
            }
            if (property.PropertyType.Name.ToLower().CompareTo("int") == 0)
            {
                property.SetValue(obj, int.Parse(value),null);
            }
            if (property.PropertyType.Name.ToLower().CompareTo("datetime") == 0)
            {
                property.SetValue(obj, DateTime.Parse(value),null);
            }
            if (property.PropertyType.Name.ToLower().CompareTo("double") == 0)
            {
                property.SetValue(obj, double.Parse(value),null);
            }
            if (property.PropertyType.Name.ToLower().CompareTo("boolean") == 0)
            {
                property.SetValue(obj, bool.Parse(value),null);
            }
        }

        public static DateTime changeDateTimeFormat(string dateString)
        {
            string[] elementsList = dateString.Split('/');
            string newFormat = elementsList[1] + "/" + elementsList[0] + "/" + elementsList[2];
            return DateTime.Parse(newFormat);
        }


    }
}
