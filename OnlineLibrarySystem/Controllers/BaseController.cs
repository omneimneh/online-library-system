using OnlineLibrarySystem.Controllers;
using OnlineLibrarySystem.Models;
using System;
using System.Web.Mvc;
using System.Web.SessionState;

namespace OnlineLibrarySystem
{
    public class BaseController : Controller
    {
        public CommonModel model = new CommonModel();

        public string CurrentActionName => ControllerContext.RouteData.Values["action"].ToString().ToLower();
        public string CurrentControllerName => ControllerContext.RouteData.Values["controller"].ToString().ToLower();

        public BaseController()
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string token = GetSession("token");
            var personType = PersonType.Student;
            if (token != null)
            {
                personType = new ApiAccountController().GetPerson(TokenManager.TokenDictionaryHolder[token]).PersonType;
            }
            model.Token = token;
            model.TokenPersonType = personType;
            if (!HasAccess(token, model.TokenPersonType, filterContext))
            {
                Session["redirect"] = Url.Action(CurrentActionName, CurrentControllerName);
                filterContext.Result = RedirectToAction("Login", "Account");
            }
        }

        private string GetSession(string key)
        {
            object val = Session[key];
            return val?.ToString();
        }

        private bool HasAccess(string token, PersonType personType, ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(token))
            {
                switch (CurrentControllerName.ToLower() + "/" + CurrentActionName.ToLower())
                {
                    case "home/index":
                    case "account/login":
                    case "account/signup":
                    case "book/search":
                    case "help/index":
                    case "help/contact":
                    case "book/index":
                        return true;
                    default: return false;
                }
            }
            else
            {
                // according to the type of this user give him access
                switch (CurrentControllerName.ToLower() + "/" + CurrentActionName.ToLower())
                {
                    case "home/index":
                    case "account/login":
                    case "account/signup":
                    case "book/search":
                    case "help/index":
                    case "help/contact":
                    case "book/index":
                        return true;
                    case "librarian/index":
                    case "librarian/checkout":
                        return personType >= PersonType.Librarian;
                    case "admin/manage":
                        return personType >= PersonType.Admin;
                    default: return true;
                }
            }
        }
    }
}