using OnlineLibrarySystem.Models;
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

        public ActionResult Checkout()
        {
            ViewBag.Title = "Librarian Checkout";
            return View(model);
        }
    }
}
