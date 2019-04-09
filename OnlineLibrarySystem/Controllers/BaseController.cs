using OnlineLibrarySystem.Models;
using System;
using System.Web.Mvc;

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

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            string token = GetSession("token");
            model.Token = token;
            if (!HasAccess(token, filterContext))
            {
                Session["redirect"] = Url.Action(CurrentActionName, CurrentControllerName);
                filterContext.Result = RedirectToAction("Login", "Account");
            }
        }

        private string GetSession(string key)
        {
            object val = Session[key];
            if (val == null) return null;
            else return val.ToString();
        }

        private bool HasAccess(string token, ActionExecutedContext context)
        {
            if (string.IsNullOrEmpty(token))
            {
                switch (CurrentControllerName.ToLower())
                {
                    case "home": case "account": return true;
                    default: return false;
                }
            }
            else
            {
                int userId = TokenManager.TokenDictionaryHolder[token];
                // according to the type of this user give him access
                switch (CurrentControllerName.ToLower())
                {
                    case "home": case "account": return true;
                    default: return true;
                }
            }
        }
    }
}