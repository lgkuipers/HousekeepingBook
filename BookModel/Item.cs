using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookModel
{
    public class Item
    {
        public string ItemName { get; set; }
        public string Tegenrekening { get; set; }
        public string Bevat { get; set; }
        public double Total { get; set; } // gets a value after processing
        public int Hits { get; set; }
        public int CategoryId { get; set; }
        public string CategoryStr { get; set; }

        public Item()
        {
            ItemName = "";
            Tegenrekening = "";
            Bevat = "";
            Total = 0.0;
            Hits = 0;
            CategoryStr = "";
            CategoryId = -1;
        }

        public void ReadLine(string aLine)
        {
            int p1, p2;
            // Line: "Post","Tegenrekening","Bevat"

            p1 = aLine.IndexOf("\"", 0);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ItemName = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            Tegenrekening = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            Bevat = aLine.Substring(p1 + 1, p2 - p1 - 1);

            Hits = 0;

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            CategoryStr = aLine.Substring(p1 + 1, p2 - p1 - 1);
        }

        public void Add2Total(double aAdd)
        {
            Total += aAdd;
        }

        public void Sub2Total(double aSub)
        {
            Total -= aSub;
        }

        public void Clear2Total()
        {
            Total = 0.0;
        }

    }
}
