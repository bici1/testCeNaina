using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace WindowsMobile.Utils
{
    public class Util
    {
        public static int GetNbPages(int count, int parpage)
        {
            int reste = count % parpage;
            int nb = count / parpage;

            if (reste > 0) nb++;

            return nb;
        }
        public static int[] GetDebFin(int numpage, int parpage)
        {
            int[] debfin = new int[2];

            debfin[0] = ((numpage - 1) * parpage) + 1;
            debfin[1] = debfin[0] + parpage - 1;

            return debfin;
        }
        public static TimeSpan FromSeconds(decimal d)
        {
            return TimeSpan.FromSeconds(decimal.ToDouble(d));
        }

        public static int ResultCount(object[] objs)
        {
            return IsEmpty(objs) ? 0 : objs.Length;
        }

        public static string ToString(TimeSpan time)
        {
            string res = "";
            bool before = false;

            if (time.Days != 0) { res += time.Days + " j "; before = true; }
            if (time.Hours != 0) { res += time.Hours + " h "; before = true; }
            if (time.Minutes != 0) { res += time.Minutes + " min "; before = true; }

            if (!before) res += time.Seconds + " s ";

            return res;
        }
        public static bool IsEmpty(string text)
        {
            return text == null || text.Replace(" ", "").Equals("");
        }

        public static bool IsEmpty(object[] tab)
        {
            return tab == null || tab.Length == 0;
        }

        public static void SetTimeZero(DateTime d)
        {
            d = d.Date;
        }
    }
}
