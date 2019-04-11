using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Login()
        {
            ViewBag.Title = "Login";
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            ViewBag.Title = "Login";
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return Login();
            }

            ApiAccountController apiController = new ApiAccountController();
            string token = apiController.Login(username, password);

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Invalid username or passowrd";
                Session.Remove("token");
            }
            else
            {
                Session["token"] = token;
                if (Session["redirect"] != null)
                {
                    string redirect = Session["redirect"].ToString();
                    Session.Remove("redirect");
                    return Redirect(redirect);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult Signup()
        {
            ViewBag.Title = "Signup";
            return View(model);
        }

        public ActionResult Logout()
        {
            if (Session["token"] != null)
            {
                TokenManager.TokenDictionaryHolder.Remove(Session["token"].ToString());
            }
            Session.Remove("token");
            return View(model);
        }
    }
}