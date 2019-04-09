using System;
using System.Web.Mvc;

namespace OnlineLibrarySystem
{
    public class BaseController : Controller
    {

        public string CurrentActionName => ControllerContext.RouteData.Values["action"].ToString().ToLower();
        public string CurrentControllerName => ControllerContext.RouteData.Values["controller"].ToString().ToLower();

        public BaseController()
        {
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            string token = GetSession("token");
            if (!HasAccess(token, filterContext))
            {
                filterContext.Result = RedirectToAction("Index", "Home");
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
                    case "home": case "login": return true;
                    default: return false;
                }
            }
            else
            {
                int userId = TokenManager.TokenDictionaryHolder[token];
                // according to the type of this user give him access
                switch (CurrentControllerName.ToLower())
                {
                    case "home": case "login": return true;
                    default: return false;
                }
            }
        }
    }
}