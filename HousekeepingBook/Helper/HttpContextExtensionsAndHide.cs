using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace HousekeepingBook.Helper
{
    public static class HttpContextExtensionsAndHide
    {
        private const string Key = "HOUSEKEEPING_BOOK_DATA";

        public static ISessionData GetDataFromSession(this HttpContext context)
        {
            return (ISessionData)context.Session[Key];
        }

        public static void SetDataToSession(this HttpContext context, ISessionData value)
        {
            context.Session[Key] = value;
        }

        public static ISessionData GetSessionData()
        {
            return HttpContext.Current.GetDataFromSession();
        }

        public static void SetSessionData(ISessionData sessionData)
        {
            HttpContext.Current.SetDataToSession(sessionData);
        }
    }


}