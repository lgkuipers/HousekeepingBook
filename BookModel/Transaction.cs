using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookModel
{
    public class Transaction
    {
        private string ivLine;
        private string ivDatum;
        private string ivNaam_Omschrijving;
        private string ivRekening;
        private string ivTegenrekening;
        private string ivCode;
        private string ivAf_Bij;
        private string ivBedrag;
        private string ivMutatieSoort;
        private string ivMededelingen;
        private int ivHit;

        public string SearchFilter { get; set; }
        public Item Item { get; set; }

        public Category Category { get; set; }
        public void ReadLine(string aLine)
        {
            int p1, p2;
            // Line: "Datum","Naam / Omschrijving","Rekening","Tegenrekening","Code","Af Bij","Bedrag (EUR)","MutatieSoort","Mededelingen"

            ivLine = aLine;

            p1 = aLine.IndexOf("\"", 0);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivDatum = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivNaam_Omschrijving = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivRekening = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivTegenrekening = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivCode = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivAf_Bij = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivBedrag = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivMutatieSoort = aLine.Substring(p1 + 1, p2 - p1 - 1);

            p1 = aLine.IndexOf("\"", p2 + 1);
            p2 = aLine.IndexOf("\"", p1 + 1);
            ivMededelingen = aLine.Substring(p1 + 1, p2 - p1 - 1);

            ivHit = 0;
        }

        public bool Bevat(string aBevat)
        {
            return (ivLine.ToUpper().IndexOf(aBevat.ToUpper()) >= 0);
        }
        public int Maand()
        {
            if (ivDatum == "Datum")
            {
                return -1;
            }
            else
            {
                string lMaand = ivDatum.Substring(4, 2); // 20140503
                return int.Parse(lMaand);
            }
        }
        public bool IsTegenrekeningEnBevat(string aTegenrekening, string aBevat)
        {
            bool lOk = false;
            if (aTegenrekening != "negeer")
            {
                lOk = (aTegenrekening == ivTegenrekening);
            }
            lOk = lOk && (ivLine.ToUpper().IndexOf(aBevat.ToUpper()) >= 0);

            return lOk;
        }
        static public string ToBedrag(double lBedrag)
        {
            string s = "";
            try
            {
                s = string.Format("{0,10:0.00}", lBedrag);
            }
            catch (Exception)
            {
                s = "-";
            }
            return s;
        }

        public override string ToString()
        {
            return ivNaam_Omschrijving;
        }

        public string Line
        {
            get { return ivLine; }
        }
        public string Datum
        {
            get { return ivDatum; }
        }
        public string Naam_Omschrijving
        {
            get { return ivNaam_Omschrijving; }
            set { ivNaam_Omschrijving = value; }
        }
        public string Rekening
        {
            get { return ivRekening; }
        }
        public string Tegenrekening
        {
            get { return ivTegenrekening; }
        }
        public string Code
        {
            get { return ivCode; }
        }
        public string Af_Bij
        {
            get { return ivAf_Bij; }
        }
        public string Bedrag
        {
            get
            {
                string s = "";
                try
                {
                    double d = double.Parse(ivBedrag);
                    s = string.Format("{0,10:0.00}", d);
                }
                catch (Exception)
                {
                    s = "-";
                }
                return s;
            }
        }
        public string MutatieSoort
        {
            get { return ivMutatieSoort; }
        }
        public string Mededelingen
        {
            get { return ivMededelingen; }
        }
        public string ToolTip
        {
            get
            {
                string toolTip =
                "Datum: " + ivDatum + Environment.NewLine +
                "Code: " + ivCode + Environment.NewLine +
                "Tegenrekening: " + ivTegenrekening + Environment.NewLine +
                "Mededeling: " + ivMededelingen;

                return toolTip;
            }
        }
        public int Hit
        {
            set { ivHit = value; }
            get { return ivHit; }
        }
    }
}
