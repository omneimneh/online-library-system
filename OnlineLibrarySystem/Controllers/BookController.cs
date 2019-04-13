using OnlineLibrarySystem.Controllers;
using OnlineLibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace OnlineLibrarySystem.Views
{
    public class BookController : BaseController
    {
        public ActionResult Search(string key = null, string searchBy = "book", int minPub = 0, int maxPub = 2020, string match = "off", int page = 1, int pageSize = 8)
        {
            ViewBag.Title = "Search";
            return View(new SearchFilter
            {
                Key = key,
                SearchBy = searchBy,
                Match = match.Equals("on"),
                MaxPub = maxPub,
                MinPub = minPub,
                Token = model.Token,
                Page = page,
                PageSize = pageSize
            });
        }

        public ActionResult Index(int bookId)
        {
            Book bookModel = new ApiBookController().Get(bookId);
            if (bookModel.BookId == -1) throw new HttpException(404, "Book not found!");
            bookModel.Token = model.Token;
            ViewBag.Title = bookModel.BookTitle;
            return View(bookModel);
        }
    }
}