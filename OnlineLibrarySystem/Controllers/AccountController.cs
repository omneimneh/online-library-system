using OnlineLibrarySystem.Models;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Login()
        {
            ViewBag.Title = "Login";
            return View(new Person
            {
                Token = model.Token,
                Error = false
            });
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
                Session.Remove("token");
                return View(new Person
                {
                    Error = true,
                    Username = username,
                    Token = model.Token
                });
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
        }

        public ActionResult Signup()
        {
            ViewBag.Title = "Signup";
            return View(new Person { Token = model.Token });
        }

        [HttpPost]
        public ActionResult Signup(string username, string password, string verifyPassword)
        {
            ViewBag.Title = "Signup";

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) && string.IsNullOrEmpty(verifyPassword))
            {
                return Signup();
            }

            Person user = new ApiAccountController().Signup(username, password, verifyPassword);
            if (user.Error)
            {
                return View(user);
            }
            Session["token"] = user.Token;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            if (Session["token"] != null)
            {
                TokenManager.TokenDictionaryHolder.Remove(Session["token"].ToString());
            }
            Session.Remove("token");
            ViewBag.Title = "Logout";
            return View(model);
        }

        public new ActionResult Profile()
        {
            ViewBag.Title = "Profile";
            var controller = new ApiAccountController();
            string token = model.Token;
            Person person = controller.GetPerson(TokenManager.TokenDictionaryHolder[token]);
            person.Token = model.Token;
            person.TokenPersonType = model.TokenPersonType;
            return View(person);
        }

        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Change Password";
            return View(model);
        }
    }
}