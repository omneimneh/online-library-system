using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Controllers
{
    public class HelpController : BaseController
    {
        // GET: Help
        public ActionResult Index()
        {
            ViewBag.Title = "Help";
            return View(model);
        }
    }
}