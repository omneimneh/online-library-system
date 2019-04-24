using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Controllers
{
    public class AdminController : BaseController
    {
        public ActionResult Manage()
        {
            ViewBag.Title = "Manage";
            return View(model);
        }
    }
}
