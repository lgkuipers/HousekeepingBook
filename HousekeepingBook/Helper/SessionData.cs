using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookModel;

namespace HousekeepingBook.Helper
{
    public class SessionData : ISessionData
    {
        public string TransactionsFile { get; set; }
        public string SearchFilter { get; set; }
        public int SearchCategory { get; set; }
        public List<Mutation4Item> MutationsList { get; set; }
    }
}