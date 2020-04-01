using HousekeepingBook.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using HousekeepingBook.Helper;
using BookModel;

namespace HousekeepingBook.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index(int? page1, int? page2, int? page3)
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            if (sessionData == null)
              HttpContextExtensionsAndHide.SetSessionData(new SessionData { TransactionsFile = "NL97INGB0006166407_01-01-2018_31-12-2018.csv", SearchFilter = "", SearchCategory = -1, MutationsList = new List<Mutation4Item>() });
            sessionData = HttpContextExtensionsAndHide.GetSessionData();


            var dataModel = new DataModel(Server.MapPath("~/App_Data/uploads"), sessionData.TransactionsFile);

            dataModel.ProcessMutations(sessionData.MutationsList);
            dataModel.SetItem4Transactions();
            dataModel.SetCategory4Transactions();
            var maand_uit = dataModel.MaandUitgaven();

            // ---

            var transactionsList = dataModel.TransactionsFilteredList;
            var itemsList = dataModel.ItemsList;

            string currentFilter = sessionData.SearchFilter;

            List<Transaction> processedTransactionList = new List<Transaction>();
            processedTransactionList = transactionsList.Where(t => t.Naam_Omschrijving.Contains(currentFilter)).ToList();

            int currentCategory = sessionData.SearchCategory;
            if ((currentCategory != -1) && (currentCategory != 0))
            {
                processedTransactionList = processedTransactionList.Where(t => t.Item.CategoryId == currentCategory).ToList();
            }

            // salaris niet steeds zichtbaar processedTransactionList = processedTransactionList.OrderByDescending(o => o.Bedrag).ToList();

            List<Transaction> unknownTransactionList = new List<Transaction>();
            unknownTransactionList = processedTransactionList.Where(t => t.Item.CategoryId == 1).OrderByDescending(o => o.Bedrag).ToList();

            CultureInfo provider = new CultureInfo("nl-NL");
            var transactionSumBij = transactionsList.Where(x => x.Af_Bij == "Bij").ToList().Sum(y => float.Parse(y.Bedrag, provider));
            var transactionSumAf = transactionsList.Where(x => x.Af_Bij == "Af").ToList().Sum(y => float.Parse(y.Bedrag, provider));
            var transactionSumSalaris = transactionsList.Where(x => (x.Af_Bij == "Bij" && (x.Naam_Omschrijving.Contains("TASS") || x.Naam_Omschrijving.Contains("Altran Netherlands") || x.Naam_Omschrijving.Contains("Altran BV") || x.Naam_Omschrijving.Contains("Nspyre B.V.")))).ToList().Sum(y => float.Parse(y.Bedrag, provider));
            var transactionSumSavingBij = transactionsList.Where(t => t.Af_Bij == "Bij" && t.Category.CategoryStr == "sparen").ToList().Sum(y => float.Parse(y.Bedrag, provider));
            var transactionSumSavingAf = transactionsList.Where(t => t.Af_Bij == "Af" && t.Category.CategoryStr == "sparen").ToList().Sum(y => float.Parse(y.Bedrag, provider));

            List<TabelRow> sum = new List<TabelRow>();
            sum.Add(new TabelRow { Column1 = "Bij", Column2 = transactionSumBij.ToString() });
            sum.Add(new TabelRow { Column1 = "Af", Column2 = transactionSumAf.ToString() });
            sum.Add(new TabelRow { Column1 = "Salary", Column2 = transactionSumSalaris.ToString() });
            sum.Add(new TabelRow { Column1 = "Sparen", Column2 = (transactionSumSavingBij - transactionSumSavingAf).ToString() });

            List<Item> processedItemList = new List<Item>();
            processedItemList = itemsList;
            if ((currentCategory != -1) && (currentCategory != 0))
            {
                processedItemList = processedItemList.Where(i => i.CategoryId == currentCategory).ToList();
            }

            dataModel.Process4Totals();

            HousekeepingBookModel model = new HousekeepingBookModel();

            int pagenr1 = (page1 ?? 1);
            int pagenr2 = (page2 ?? 1);
            int pagenr3 = (page3 ?? 1);

            model.transactionsFile = Path.GetFileName(sessionData.TransactionsFile);
            model.transactionList = processedTransactionList;
            model.sumList = sum.ToPagedList(pagenr2, 3);
            model.itemList = processedItemList;
            model.errorList = dataModel.ErrorsList;
            model.categoryList = dataModel.CategoriesList;
            model.unknownList = unknownTransactionList;

            model.data = maand_uit;
            model.labels = new List<string> { "jan", "feb", "maart", "april", "mei", "juni", "juli", "aug", "sep", "okt", "nov", "dec" };

            model.categorySelectList = new List<SelectListItem>();
            foreach(var o in dataModel.CategoriesList)
            {
                SelectListItem sli = new SelectListItem { Text = o.CategoryStr, Value = o.CategoryId.ToString(), Selected = false };
                model.categorySelectList = model.categorySelectList.Concat(new [] { sli });
            }

            model.CategoryId = sessionData.SearchCategory;

            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);

                sessionData.TransactionsFile = fileName;
                HttpContextExtensionsAndHide.SetSessionData(sessionData);

            }
            return RedirectToAction("Index");
        }

        public ActionResult SaveMutations()
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();
            var dataModel = new DataModel(Server.MapPath("~/App_Data/uploads"), sessionData.TransactionsFile);

            dataModel.ProcessMutations(sessionData.MutationsList);
            dataModel.SetItem4Transactions();
            dataModel.SetCategory4Transactions();

            dataModel.ProcessSaveMutations(sessionData.MutationsList);

            return RedirectToAction("Index");
        }

        public ActionResult ResetFilters()
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            var initial = new SessionData { TransactionsFile = "", SearchFilter = "", SearchCategory = -1, MutationsList = new List<Mutation4Item>() };

            sessionData.SearchFilter = initial.SearchFilter;
            sessionData.SearchCategory = initial.SearchCategory;

            HttpContextExtensionsAndHide.SetSessionData(sessionData);

            return RedirectToAction("Index");
        }

        public ActionResult ResetYear(int Year)
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            var initial = new SessionData { TransactionsFile = "NL97INGB0006166407_01-01-2018_31-12-2018.csv", SearchFilter = "", SearchCategory = -1, MutationsList = new List<Mutation4Item>() };

            string yearStr = Year.ToString();
            initial.TransactionsFile = initial.TransactionsFile.Replace("2018", yearStr);

            sessionData.TransactionsFile = initial.TransactionsFile;
            sessionData.SearchFilter = initial.SearchFilter;
            sessionData.SearchCategory = initial.SearchCategory;

            HttpContextExtensionsAndHide.SetSessionData(sessionData);

            return RedirectToAction("Index");
        }

        public ActionResult DisplayTable(int? Year)
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            var initial = new SessionData { TransactionsFile = "NL97INGB0006166407_01-01-2018_31-12-2018.csv", SearchFilter = "", SearchCategory = -1, MutationsList = new List<Mutation4Item>() };

            // string yearStr = Year.ToString();
            // initial.TransactionsFile = initial.TransactionsFile.Replace("2018", yearStr);

            // sessionData.TransactionsFile = initial.TransactionsFile;
            // sessionData.SearchFilter = initial.SearchFilter;
            // sessionData.SearchCategory = initial.SearchCategory;

            // HttpContextExtensionsAndHide.SetSessionData(sessionData);

            HousekeepingBookModel model = new HousekeepingBookModel();

            model.transactionsFile = "";
            model.transactionList = new List<Transaction>();
            model.sumList = (new List<TabelRow>()).ToPagedList(1, 3);
            model.itemList = new List<Item>();
            model.errorList = new List<string>();
            model.categoryList = new List<Category>();
            model.unknownList = new List<Transaction>();

            model.data = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            model.labels = new List<string> { "jan", "feb", "maart", "april", "mei", "juni", "juli", "aug", "sep", "okt", "nov", "dec" };

            model.categorySelectList = new List<SelectListItem>();

            return PartialView("DisplayTable", model);
        }

        [HttpPost]
        public ActionResult Search(HousekeepingBookModel model, string currentFilter)
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            sessionData.SearchFilter = currentFilter;
            sessionData.SearchCategory = model.CategoryId;

            HttpContextExtensionsAndHide.SetSessionData(sessionData);

            return RedirectToAction("Index");
        }
        public ActionResult SaveItem(HousekeepingBookModel model, string itemValue, string rowIndex)
        {
            var sessionData = HttpContextExtensionsAndHide.GetSessionData();

            sessionData.MutationsList.Add(new Mutation4Item() { ItenNr = int.Parse(rowIndex) - 1, ItemValue = int.Parse(itemValue) });

            HttpContextExtensionsAndHide.SetSessionData(sessionData);

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Housekeeping Book V1.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Lolke Kuipers.";

            return View();
        }
    }
}