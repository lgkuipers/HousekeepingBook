using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookModel
{
    public class Category
    {
        public string CategoryStr { get; set; }
        public int CategoryId { get; set; }
        public double TotalBij { get; set; } // gets a value after processing
        public double TotalAf { get; set; } // gets a value after processing
        public double Total { get; set; } // gets a value after processing

        public void Dummy()
        {
            double d;

            d = 12.345;
            var s = d.FormatDouble();

            var s2 = Total.FormatDouble();
        }
    }

    public static class DoubleExtensions
    {
        public static string FormatDouble(this double d)
        {
            var s = d.ToString("N", new CultureInfo("is-IS"));
            return s;
        }
    }
}
