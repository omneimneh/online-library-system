using OnlineLibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnlineLibrarySystem.Controllers
{
    public class ApiBookController : ApiController
    {
        [HttpGet]
        [Route("api/ApiBook/GetMostPopular")]
        public List<Book> GetMostPopular(int count)
        {
            using (SqlDataReader reader = DB.ExecuteQuery(
                    "SELECT TOP(@count) * FROM Book LEFT OUTER JOIN " +
                    "(SELECT BookId, COUNT(ReservationId) AS ReservationCount FROM Reservation GROUP BY BookId) " +
                    "AS Res ON Book.BookId = Res.BookId ORDER BY ReservationCount DESC",
                new KeyValuePair<string, object>("count", count)))
            {
                List<Book> retVal = new List<Book>();
                while (reader.Read())
                {
                    retVal.Add(new Book
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        BookTitle = reader["BookTitle"].ToString(),
                        BookDescription = reader["BookDescription"]?.ToString(),
                        AuthorName = reader["AuthorName"]?.ToString(),
                        PublishingDate = Convert.ToDateTime(reader["PublishingDate"]),
                        ThumbnailImage = reader["ThumbnailImage"]?.ToString()
                    });
                }

                return retVal;
            }
        }

        [HttpGet]
        [Route("api/ApiBook/Get")]
        public Book Get(int bookId)
        {
            using (SqlDataReader reader = DB.ExecuteQuery("Select * FROM Book WHERE BookId = @id",
                new KeyValuePair<string, object>("id", bookId)))
            {
                if (reader.Read())
                {
                    return new Book
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        AuthorName = reader["AuthorName"]?.ToString(),
                        BookDescription = reader["BookDescription"]?.ToString(),
                        BookTitle = reader["BookTitle"].ToString(),
                        PublishingDate = Convert.ToDateTime(reader["PublishingDate"]),
                        ThumbnailImage = reader["ThumbnailImage"].ToString()
                    };
                }
                else
                {
                    return new Book { BookId = -1 };
                }
            }
        }
    }
}
