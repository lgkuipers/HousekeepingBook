using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using BookModel;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HousekeepingBook.Models
{
    public class HousekeepingBookModel
    {
        public string transactionsFile { get; set; }
        public List<Transaction> transactionList { get; set; }
        public IPagedList<TabelRow> sumList { get; set; }

        public List<Item> itemList { get; set; }
        public List<Category> categoryList { get; set; }
        public List<string> errorList { get; set; }

        public List<int> data { get; set; }
        public List<string> labels { get; set; }
        public List<Transaction> unknownList { get; set; }

        public IEnumerable<SelectListItem> categorySelectList { get; set; }

        public int CategoryId { get; set; }

        public Item PartialItem { get; set; }
        public int PartialItemId { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "This Field is required")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This Field is required")]
        public string Password { get; set; }

        public string LoginErrorMsg { get; set; }
    }
}