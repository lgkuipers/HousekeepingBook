using HousekeepingBook.Models;
using HousekeepingBook.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HousekeepingBook.Controllers
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Authorise(HousekeepingBookModel model)
        {
            List<User> allowedUsers = new List<User> {
                new User { UserID = 1, UserName = "admin", Password = "E1mS3zQ9YnaJ4miAn0i92w==" },
                new User { UserID = 2, UserName = "lolke", Password = "tFNhS17FlrF1jzZQYNP96A==" },
                new User { UserID = 3, UserName = "geert", Password = "Uj/jKZIaLHcw40QUdb3UiA==" }
            };

            //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(allowedUsers, Newtonsoft.Json.Formatting.Indented);
            //System.IO.File.WriteAllText(Path.Combine(Server.MapPath("~/App_data"), "users.json"), jsonString);

            if (System.IO.File.Exists(Path.Combine(Server.MapPath("~/App_data"), "users.json")))
            {
                var jsonString2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~/App_data"), "users.json"));
                allowedUsers = (List<User>)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString2, typeof(List<User>));
            }

            var userDetail = allowedUsers.Where(x => x.UserName == model.UserName && x.Password == Encrypt.EncryptString(model.Password, "Altran")).FirstOrDefault();

            if (userDetail == null)
            {
                model.LoginErrorMsg = "Invalid UserName or Password";
                return View("Index", model);
            }
            else
            {
                Session["userID"] = userDetail.UserID;
                Session["userName"] = userDetail.UserName;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}