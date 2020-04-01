using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.IO;

namespace BookModel
{
    public class DataModel
    {
        public List<Transaction> TransactionsFilteredList { get; set; }
        public List<Item> ItemsList { get; set; }
        private string pathServer;
        private string fileNameTransactions;

        public List<string> ErrorsList { get; set; }

        public List<Category> CategoriesList { get; set; }
        public DataModel(string path1, string transactionsFile)
        {
            pathServer = path1;
            fileNameTransactions = transactionsFile;
            ErrorsList = new List<string>();
            LoadTransactions();
            LoadCategories();
            LoadItems();
            SetItem4Transactions();
            SetCategory4Transactions();
        }
        public void LoadTransactions()
        {
            List<string> lines = new List<string>();
            lines.Add("\"20151231\",\"STICHTING FOUNDATION DOCDATA\",\"NL97INGB0006166407\",\"NL95DEUT0410187526\",\"GT\",\"Af\",\"9,99\",\"Internetbankieren\",\"IBAN: NL95DEUT0410187526 BIC: DEUTNL2N Naam: STICHTING FOUNDATION DOCDATA Kenmerk: 31-12-2015 17:33 1150000478684128 Omschrijving: 2640125630 1150000478684128 pid2640125630t Veen Media Veen Magazines\"");
            lines.Add("\"20151231\",\"TEXACO NUENEN NUENEN NLD\",\"NL97INGB0006166407\",\"\",\"BA\",\"Af\",\"61,27\",\"Betaalautomaat\",\"Pasvolgnr:013 30-12-2015 11:19 Transactie:75W0W5 Term:10033611\"");
            lines.Add("\"20151231\",\"STICHTING LEDENSERVICE ZUIDZOR G\",\"NL97INGB0006166407\",\"NL02INGB0651034205\",\"IC\",\"Af\",\"19,49\",\"Incasso\",\"Europese Incasso, doorlopend IBAN: NL02INGB0651034205 BIC: INGBNL2A Naam: STICHTING LEDENSERVICE ZUIDZOR G ID begunstigde: NL83ZZZ402402310000 SEPA ID machtiging: 10326258-322084 Kenmerk: 0000600007140000711000349683RCUR Omschrijving: Lidmaatschap ZuidZorg Extra 2016, bij vragen bel: 040 - 2 308 538\"");
            lines.Add("\"20151231\",\"FLORIUS\",\"NL97INGB0006166407\",\"NL33RABO0100926797\",\"IC\",\"Af\",\"540,78\",\"Incasso\",\"Europese Incasso, doorlopend IBAN: NL33RABO0100926797 BIC: RABONL2U Naam: FLORIUS ID begunstigde: NL42ZZZ080242850000 SEPA ID machtiging: ST00100005671450001 Kenmerk: 0000076001038421 Omschrijving: Verschuldigde bedragen PERIODE 12-2015\"");
            lines.Add("\"20151231\",\"THIS GENERATION\",\"NL97INGB0006166407\",\"NL61INGB0008073978\",\"IC\",\"Af\",\"15,00\",\"Incasso\",\"Europese Incasso, doorlopend IBAN: NL61INGB0008073978 BIC: INGBNL2A Naam: THIS GENERATION ID begunstigde: NL08ZZZ999999992830 SEPA ID machtiging: TG022 Kenmerk: DD-20151220-3MRJKCP9-0022 Omschrijving: Contributie This Generation december\"");
            TransactionsFilteredList = new List<Transaction>();
            foreach (var l in lines)
            {
                var transaction = new Transaction();
                transaction.ReadLine(l);

                TransactionsFilteredList.Add(transaction);
            }

            string fileName = "NL97INGB0006166407_01-01-2015_31-12-2015.csv";
            var path = Path.Combine(pathServer, fileName);

            if (fileNameTransactions != "")
            {
                path = Path.Combine(pathServer, fileNameTransactions);
            }

            if (File.Exists(path))
            {
                TransactionsFilteredList = ReadListOfTransactions(path);
            }
        }

        public void LoadItems()
        {
            string fileName = "posten.csv";
            var path = Path.Combine(pathServer, fileName);
            if (File.Exists(path))
            {
                ItemsList = ReadListOfItems(path);
            }
        }

        public void LoadCategories()
        {
            string fileName = "posten.csv";
            var path = Path.Combine(pathServer, fileName);
            if (File.Exists(path))
            {
                CategoriesList = ReadListOfCategories(path);
            }
        }

        public void SetItem4Transactions()
        {
            string itemListNewFile = "posten_nieuw.csv";
            var path = Path.Combine(pathServer, itemListNewFile);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            int count = 0;
            int volgnummer = 0;
            List<string> lListNaamOmschrijving = new List<string>();

            foreach (var t in TransactionsFilteredList)
            {
                bool lFound = false;
                foreach (var p in ItemsList) // transaction of known item gets last found item (last item can overrule previous items)
                {
                    if (t.IsTegenrekeningEnBevat(p.Tegenrekening, p.Bevat))
                    {
                        lFound = true;
                        t.Item = p;
                    }
                }
                if (!lFound) // transaction of unkonw item get item unknown
                {
                    Item i = ItemsList[0]; // item unknown
                    t.Item = i;
                    t.Item.CategoryId = CategoriesList[1].CategoryId; // category unknown
                    t.Item.CategoryStr = CategoriesList[1].CategoryStr;

                    count++;
                    ErrorsList.Add(t.ToString() + "-- transaction not found in itemslist; set to item unknown");

                    if (!lListNaamOmschrijving.Contains(t.Naam_Omschrijving))
                    {
                        lListNaamOmschrijving.Add(t.Naam_Omschrijving);

                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("");
                            //              item                                      tegenrekening                         bevat                                     category
                            sw.Write("\"" + t.Naam_Omschrijving + "\"" + "," + "\"" + t.Tegenrekening + "\"" + "," + "\"" + t.Naam_Omschrijving + "\"" + "," + "\"" + "unknown" + "\"");
                            sw.Close();
                            volgnummer++;
                        }
                    }

                }
            }
        }

        public void SetCategory4Transactions()
        {
            foreach (var t in TransactionsFilteredList)
            {
                bool lFound = false;
                foreach (var i in ItemsList)
                {
                    if (t.Item.CategoryId == i.CategoryId)
                    {
                        lFound = true;
                        t.Category = new Category() { CategoryId = i.CategoryId, CategoryStr = i.CategoryStr };
                    }
                }
                if (!lFound)
                {
                    ErrorsList.Add(t.ToString() + "-- not to category");
                }
            }
        }

        public List<int> MaandUitgaven()
        {
            var IdBijzonderBijzonder = CategoriesList.Where(c => c.CategoryStr == "bijzonder").First().CategoryId;
            var IdBijzonderSparen = CategoriesList.Where(c => c.CategoryStr == "sparen").First().CategoryId;
            CultureInfo provider = new CultureInfo("nl-NL");
            var maand_uit = new List<int>();
            for (int i=0; i<12; i++)
            {
                maand_uit.Add(0);
            }
            foreach (var t in TransactionsFilteredList)
            {
                if ((t.Af_Bij == "Af") && (t.Category.CategoryId != IdBijzonderBijzonder) && (t.Category.CategoryId != IdBijzonderSparen))
                {
                    try
                    {
                        var maand = int.Parse(t.Datum.Substring(4, 2)) - 1;
                        maand_uit[maand] += (int) double.Parse(t.Bedrag, provider);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    int i = 10;
                }
            }
            return maand_uit;
        }

        public void Process4Totals()
        {
            Process4TotalsItems();
            Process4TotalsCategories();
        }

        public void Process4TotalsItems()
        {
            // verzamel de posten
            // maak alle csv posten 0

            foreach (var item in ItemsList)
            {
                item.Total = 0.0; // maak de csv post 0
            }

            // verzamel de bij/af bedragen per csv post

            foreach (var transaction in TransactionsFilteredList)
            {
                bool lFound = false;
                foreach (var item in ItemsList)
                {
                    if (transaction.IsTegenrekeningEnBevat(item.Tegenrekening, item.Bevat))
                    {
                        lFound = true;
                        try
                        {
                            if (transaction.Af_Bij == "Bij")
                            {
                                item.Total += double.Parse(transaction.Bedrag);
                            }
                            else if (transaction.Af_Bij == "Af")
                            {
                                item.Total -= double.Parse(transaction.Bedrag);
                            }
                        }
                        catch (Exception)
                        {
                            ErrorsList.Add(transaction.Naam_Omschrijving + " " + "error in double parse");
                        }
                    }
                }
                if (!lFound)
                {
                    var item = ItemsList[0]; // item unkown
                    if (transaction.Item.CategoryId == CategoriesList[1].CategoryId) // collect unknown
                    {
                        if (transaction.Af_Bij == "Bij")
                        {
                            item.Total += double.Parse(transaction.Bedrag);
                        }
                        else if (transaction.Af_Bij == "Af")
                        {
                            item.Total -= double.Parse(transaction.Bedrag);
                        }
                    }
                    ErrorsList.Add(transaction.ToString() + "-- not to item");
                }
            }

            // post: ivListOfCSVPosten is gevuld

        }
        public void Process4TotalsCategories()
        {
            // verzamel de posten
            // maak alle csv posten 0

            foreach (var category in CategoriesList)
            {
                category.Total = 0.0; // maak de csv post 0
                category.TotalAf = 0.0;
                category.TotalBij = 0.0;
            }

            // verzamel de bij/af bedragen per csv post

            foreach (var transaction in TransactionsFilteredList)
            {
                bool lFound = false;
                foreach (var category in CategoriesList)
                {
                    if (transaction.Category.CategoryId == category.CategoryId)
                    {
                        lFound = true;
                        try
                        {
                            if (transaction.Af_Bij == "Bij")
                            {
                                category.Total += double.Parse(transaction.Bedrag);
                                category.TotalBij += double.Parse(transaction.Bedrag);
                            }
                            else if (transaction.Af_Bij == "Af")
                            {
                                category.Total -= double.Parse(transaction.Bedrag);
                                category.TotalAf -= double.Parse(transaction.Bedrag);
                            }
                        }
                        catch (Exception)
                        {
                            ErrorsList.Add(transaction.Naam_Omschrijving + " " + "error in double parse");
                        }
                    }
                }
                if (!lFound)
                {
                    ErrorsList.Add(transaction.ToString() + "-- not to item");
                }

                var category2 = CategoriesList[0]; // not selected
                try
                {
                    if (transaction.Af_Bij == "Bij")
                    {
                        category2.Total += double.Parse(transaction.Bedrag);
                        category2.TotalBij += double.Parse(transaction.Bedrag);
                    }
                    else if (transaction.Af_Bij == "Af")
                    {
                        category2.Total -= double.Parse(transaction.Bedrag);
                        category2.TotalAf -= double.Parse(transaction.Bedrag);
                    }
                }
                catch(Exception ex)
                { }
            }

            // post: ivListOfCSVPosten is gevuld

        }
        public List<Transaction> ReadListOfTransactions(string transactionsFile)
        {
            List<Transaction> lListOfTransactions = new List<Transaction>();
            string lLine;
            int lCounter = 0;
            bool lFirst = true;

            lListOfTransactions.Clear();

            System.IO.StreamReader file =
               new System.IO.StreamReader(transactionsFile);
            while ((lLine = file.ReadLine()) != null)
            {
                Transaction lTransactie = new Transaction();
                lTransactie.ReadLine(lLine);

                if (lFirst )
                {
                    lFirst = false; // skip first line of transactions
                }
                else
                {
                    lListOfTransactions.Add(lTransactie);
                }

                lCounter++;
            }
            file.Close();

            return lListOfTransactions;
        }

        private int Convert2Id(string categroy)
        {
            foreach (var o in CategoriesList)
            {
                if (o.CategoryStr == categroy)
                {
                    return o.CategoryId;
                }
            }
            return -1;
        }

        private string Convert2Str(int categoryId)
        {
            foreach (var o in CategoriesList)
            {
                if (o.CategoryId == categoryId)
                {
                    return o.CategoryStr;
                }
            }
            return "---";
        }

        public List<Item> ReadListOfItems(string itemsFile)
        {
            List<Item> lListOfItems = new List<Item>();
            List<Item> lItemsToSkip = new List<Item> { new Item { ItemName = "Post" }, new Item { ItemName = "category" }, new Item { ItemName = "comment" } };
            string lLine;
            int lCounter = 0;

            lListOfItems.Clear();

            lListOfItems.Add(new Item() { ItemName = "unknown", Tegenrekening = "xxx", Bevat = "xxx", CategoryId = 0, CategoryStr = "unknown" });

            System.IO.StreamReader file =
               new System.IO.StreamReader(itemsFile);
            while ((lLine = file.ReadLine()) != null)
            {
                Item lItem = new Item();
                lItem.ReadLine(lLine);

                if (!lItemsToSkip.Exists(x => x.ItemName == lItem.ItemName))
                {
                    lItem.CategoryId = Convert2Id(lItem.CategoryStr);

                    if (lListOfItems.Exists(i => i.ItemName == lItem.ItemName))
                    {
                        ErrorsList.Add(lItem.ItemName + " " + lItem.Bevat + " " + "-- already exists");
                    }

                    lListOfItems.Add(lItem);

                    lCounter++;
                }
                else
                {
                    // skip item
                    int i = 1;
                }
            }
            file.Close();

            return lListOfItems;
        }

        public List<Category> ReadListOfCategories(string itemsFile)
        {
            List<Category> lListOfCategories = new List<Category>();
            string lLine;
            int lCounter = 0;

            lListOfCategories.Clear();

            lListOfCategories.Add(new Category() { CategoryId = 0, CategoryStr = "not selected", Total = 0 });
            lCounter++;
            lListOfCategories.Add(new Category() { CategoryId = 1, CategoryStr = "unknown", Total = 0 });

            System.IO.StreamReader file =
               new System.IO.StreamReader(itemsFile);
            while ((lLine = file.ReadLine()) != null)
            {
                Item lPost = new Item();
                lPost.ReadLine(lLine);

                var categorystr = lPost.CategoryStr;

                bool lFound = false;
                foreach (var o in lListOfCategories)
                {
                    if (o.CategoryStr == categorystr)
                    {
                        lFound = true;
                    }
                }

                if (!lFound)
                {
                    if (lPost.ItemName != "comment")
                    {
                        lCounter++;
                        Category o = new Category();
                        o.CategoryId = lCounter; // overview item gets a value (id)
                        o.CategoryStr = categorystr;
                        lListOfCategories.Add(o);
                    }
                }
            }
            file.Close();

            return lListOfCategories;
        }

        public void ProcessMutations(List<Mutation4Item> mutations)
        {
            foreach(var m in mutations)
            {
                ItemsList[m.ItenNr].CategoryId = m.ItemValue;
                ItemsList[m.ItenNr].CategoryStr = Convert2Str(m.ItemValue);
            }
        }
        public void ProcessSaveMutations(List<Mutation4Item> mutations)
        {
            // dump ItemsList

            string itemListDumpFile = "posten_dump.csv";
            var path = Path.Combine(pathServer, itemListDumpFile);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            bool lSkipFirstWriteLine = true;
            foreach (var item in ItemsList)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    if (lSkipFirstWriteLine)
                    {
                        lSkipFirstWriteLine = false;
                    }
                    else
                    {
                        sw.WriteLine("");
                    }
                    sw.Write(
                        "\"" + item.ItemName + "\"" + "," +
                        "\"" + item.Tegenrekening + "\"" + "," +
                        "\"" + item.Bevat + "\"" + "," +
                        "\"" + item.CategoryStr + "\"");
                    sw.Close();
                }
            }

        }
    }
}