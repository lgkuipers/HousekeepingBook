using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookModel;

namespace HousekeepingBook.Helper
{
    public interface ISessionData
    {
        string TransactionsFile { get; set; }
        string SearchFilter { get; set; }
        int SearchCategory { get; set; }
        List<Mutation4Item> MutationsList { get; set; }
    }
}