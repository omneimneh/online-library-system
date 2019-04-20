using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Controllers
{
    public class LibrarianController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Librarian Dashboard";
            return View(model);
        }
    }
}
