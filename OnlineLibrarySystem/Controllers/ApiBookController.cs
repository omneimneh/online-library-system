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
                    "SELECT TOP(@count) * FROM BookInfo LEFT OUTER JOIN (SELECT BookId, COUNT(ReservationId) " +
                    "AS ReservationCount FROM Reservation GROUP BY BookId) " +
                    "AS Res ON BookInfo.BookId = Res.BookId ORDER BY ReservationCount DESC",
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
                        PublishingDate = Convert.ToDateTime(reader["PublishingDate"]).ToString("MM/dd/yyyy"),
                        ThumbnailImage = reader["ThumbnailImage"]?.ToString(),
                        PublisherName = reader["PublisherName"]?.ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"])
                    });
                }

                return retVal;
            }
        }

        [HttpGet]
        [Route("api/ApiBook/Search")]
        public SearchResult Search(string searchBy, int minPub, int maxPub, int page, int pageSize,
            string key = null, string match = "off")
        {
            SearchResult retVal = new SearchResult { Results = new List<Book>() };

            if (string.IsNullOrEmpty(key)) return retVal;

            string byBook = searchBy.Equals("book") ? "1=0" : "1=1";
            string byAuthor = searchBy.Equals("author") ? "1=0" : "1=1";
            string byPublisher = searchBy.Equals("publisher") ? "1=0" : "1=1";
            int start = (page - 1) * pageSize + 1;
            int end = start + pageSize - 1;

            List<KeyValuePair<string, object>> @params = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("key", key),
                new KeyValuePair<string, object>("min", minPub.ToString()),
                new KeyValuePair<string, object>("max", maxPub.ToString()),
                new KeyValuePair<string, object>("start", start),
                new KeyValuePair<string, object>("end", end)
            };

            string searchQuery = string.Format("SELECT * FROM ( SELECT ROW_NUMBER() OVER(ORDER BY[Count]) AS[Row], " +
                "BookInfo.* FROM BookInfo LEFT OUTER JOIN(Select COUNT(BookId) AS[Count], BookId FROM Reservation GROUP " +
                "BY BookId) AS Res ON BookInfo.BookId = Res.BookId WHERE (BookTitle LIKE CONCAT('{3}',@key,'{3}') OR {0}) " +
                "AND(AuthorName LIKE CONCAT('{3}',@key,'{3}') OR {1}) AND (PublisherName LIKE CONCAT('{3}',@key,'{3}') OR " +
                "{2}) AND YEAR(PublishingDate) BETWEEN @min AND @max) AS Result WHERE[Row] BETWEEN @start AND @end",
                byBook, byAuthor, byPublisher, match.Equals("on") ? "" : "%");

            using (SqlDataReader reader = DB.ExecuteQuery(searchQuery, @params[0], @params[1], @params[2], @params[3], @params[4]))
            {
                while (reader.Read())
                {
                    retVal.Results.Add(new Book
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        AuthorName = reader["AuthorName"]?.ToString(),
                        BookTitle = reader["BookTitle"].ToString(),
                        BookDescription = reader["BookDescription"]?.ToString(),
                        PublishingDate = Convert.ToDateTime(reader["PublishingDate"]).ToString("MM/dd/yyyy"),
                        ThumbnailImage = reader["ThumbnailImage"].ToString(),
                        PublisherName = reader["PublisherName"]?.ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"])
                    });
                }
            }

            string countQuery = string.Format("SELECT COUNT(*) FROM BookInfo LEFT OUTER JOIN(Select COUNT(BookId) AS[Count], " +
                "BookId FROM Reservation GROUP BY BookId) AS Res ON BookInfo.BookId = Res.BookId WHERE (BookTitle LIKE " +
                "CONCAT('{3}',@key,'{3}') OR {0}) AND (AuthorName LIKE CONCAT('{3}',@key,'{3}') OR {1}) AND (PublisherName " +
                "LIKE CONCAT('{3}',@key,'{3}') OR {2}) AND YEAR(PublishingDate) BETWEEN @min AND @max",
                byBook, byAuthor, byPublisher, match.Equals("on") ? "" : "%");

            retVal.TotalCount = DB.ExecuteScalar(countQuery, @params[0], @params[1], @params[2]);

            return retVal;
        }

        [HttpGet]
        [Route("api/ApiBook/Get")]
        public Book Get(int bookId)
        {
            using (SqlDataReader reader = DB.ExecuteQuery("Select * FROM BookInfo WHERE BookId = @id",
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
                        PublishingDate = Convert.ToDateTime(reader["PublishingDate"]).ToString("MM/dd/yyyy"),
                        ThumbnailImage = reader["ThumbnailImage"].ToString(),
                        PublisherName = reader["PublisherName"]?.ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"])
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
