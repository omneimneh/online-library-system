using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Views
{
    public class BooksController : BaseController
    {
        // GET: Book
        public ActionResult Search()
        {
            ViewBag.Title = "Search";
            return View(model);
        }
    }
}