using OnlineLibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;

namespace OnlineLibrarySystem.Controllers
{
    public class ApiBookController : ApiController
    {
        [HttpGet]
        [Route("api/ApiBook/GetMostPopular")]
        public List<Book> GetMostPopular(int count)
        {
            List<Book> retVal = new List<Book>();
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (SqlDataReader reader = DB.ExecuteQuery(con,
                    "SELECT TOP(@count) * FROM BookInfo LEFT OUTER JOIN (SELECT BookId, COUNT(ReservationId) " +
                    "AS ReservationCount FROM Reservation GROUP BY BookId) " +
                    "AS Res ON BookInfo.BookId = Res.BookId ORDER BY ReservationCount DESC",
                new KeyValuePair<string, object>("count", count)))
                {
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
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            NextAvailable = ExtractDate(reader["NextAvailable"])
                        });
                    }
                }
            }
            return retVal;
        }

        private static string ExtractDate(object dateTime)
        {
            try
            {
                return Convert.ToDateTime(dateTime).ToString("MM/dd/yyyy");
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("api/ApiBook/Search")]
        public SearchResult Search(string searchBy, int minPub, int maxPub, int page, int pageSize,
            string key = null, string match = "off")
        {
            SearchResult retVal = new SearchResult { Results = new List<Book>() };

            if (key == null) key = "";

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
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                string searchQuery = string.Format("SELECT * FROM ( SELECT ROW_NUMBER() OVER(ORDER BY[Count]) AS[Row], " +
                "BookInfo.* FROM BookInfo LEFT OUTER JOIN(Select COUNT(BookId) AS[Count], BookId FROM Reservation GROUP " +
                "BY BookId) AS Res ON BookInfo.BookId = Res.BookId WHERE (BookTitle LIKE CONCAT('{3}',@key,'{3}') OR {0}) " +
                "AND(AuthorName LIKE CONCAT('{3}',@key,'{3}') OR {1}) AND (PublisherName LIKE CONCAT('{3}',@key,'{3}') OR " +
                "{2}) AND (PublishingDate IS NULL OR YEAR(PublishingDate) BETWEEN @min AND @max)) AS Result WHERE[Row] BETWEEN @start AND @end",
                byBook, byAuthor, byPublisher, match.Equals("on") ? "" : "%");

                using (SqlDataReader reader = DB.ExecuteQuery(con, searchQuery, @params[0], @params[1], @params[2], @params[3], @params[4]))
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
                    "LIKE CONCAT('{3}',@key,'{3}') OR {2}) AND (PublishingDate IS NULL OR YEAR(PublishingDate) BETWEEN @min AND @max)",
                    byBook, byAuthor, byPublisher, match.Equals("on") ? "" : "%");

                retVal.TotalCount = DB.ExecuteScalar(con, countQuery, @params[0], @params[1], @params[2]);
            }
            return retVal;
        }

        [HttpGet]
        [Route("api/ApiBook/Get")]
        public Book Get(int bookId)
        {
            Book retVal = null; using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (SqlDataReader reader = DB.ExecuteQuery(con, "Select * FROM BookInfo WHERE BookId = @id",
                new KeyValuePair<string, object>("id", bookId)))
                {
                    if (reader.Read())
                    {
                        retVal = new Book
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            AuthorName = reader["AuthorName"]?.ToString(),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            BookTitle = reader["BookTitle"].ToString(),
                            PublishingDate = ExtractDate(reader["PublishingDate"]),
                            ThumbnailImage = reader["ThumbnailImage"].ToString(),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            NextAvailable = ExtractDate(reader["NextAvailable"])
                        };
                    }
                    else
                    {
                        retVal = new Book { BookId = -1 };
                    }
                }
            }
            return retVal;
        }

        [HttpPost]
        [Route("api/ApiBook/Rent")]
        public bool Rent([FromBody]RentRequest rentRequest)
        {
            var pickupDateStr = rentRequest.PickupDateStr;
            var bookId = rentRequest.BookId;
            var token = rentRequest.Token;
            DateTime pickupDate = DateTime.Parse(pickupDateStr);
            int userId = TokenManager.TokenDictionaryHolder[token];
            if (userId < 0) return false;
            if (pickupDate < DateTime.Now.AddDays(-1) || pickupDate > DateTime.Now.AddDays(7)) return false;

            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (SqlDataReader reader = DB.ExecuteQuery(con, "SELECT * FROM BookInfo WHERE BookId = @id",
                new KeyValuePair<string, object>("id", bookId)))
                {
                    if (reader.Read())
                    {
                        if (Convert.ToInt32(reader["Quantity"]) < 1) return false;
                    }
                    else return false;
                }

                DB.ExecuteNonQuery(con, "INSERT INTO Reservation (BookId, PickupDate, ReturnDate, IsDone, PersonId, Quantity) " +
                    "VALUES(@bookId, @date, @retDate, 0, @personId, 1)", new KeyValuePair<string, object>("bookId", bookId),
                    new KeyValuePair<string, object>("date", pickupDate),
                    new KeyValuePair<string, object>("retDate", pickupDate.AddDays(14)),
                    new KeyValuePair<string, object>("personId", userId));
            }

            return true;
        }

        [HttpGet]
        [Route("api/ApiBook/GetBooks")]
        public List<Book> GetBooks()
        {
            List<Book> retVal = new List<Book>();
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (var reader = DB.ExecuteQuery(con, "SELECT * FROM Book LEFT OUTER JOIN Author ON Author.AuthorId = Book.AuthorId " +
                "LEFT OUTER JOIN Publisher ON Publisher.Publisherid = Book.PublisherId"))
                {
                    while (reader.Read())
                    {
                        retVal.Add(new Book
                        {
                            BookTitle = reader["BookTitle"].ToString(),
                            AuthorName = reader["AuthorName"]?.ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            PublishingDate = ExtractDate(reader["PublishingDate"])
                        });
                    }
                }
            }
            return retVal;
        }

        [HttpGet]
        [Route("api/ApiBook/GetAuthors")]
        public List<Author> GetAuthors()
        {
            List<Author> retVal = new List<Author>();
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (var reader = DB.ExecuteQuery(con, "SELECT * FROM Author"))
                {
                    while (reader.Read())
                    {
                        retVal.Add(new Author
                        {
                            AuthorId = Convert.ToInt32(reader["AuthorId"]),
                            AuthorName = reader["AuthorName"]?.ToString()
                        });
                    }
                }
            }
            return retVal;
        }

        [HttpGet]
        [Route("api/ApiBook/GetPublishers")]
        public List<Publisher> GetPublishers()
        {
            List<Publisher> retVal = new List<Publisher>();
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (var reader = DB.ExecuteQuery(con, "SELECT * FROM Publisher"))
                {
                    while (reader.Read())
                    {
                        retVal.Add(new Publisher
                        {
                            PublisherId = Convert.ToInt32(reader["PublisherId"]),
                            PublisherName = reader["PublisherName"]?.ToString()
                        });
                    }
                }
            }
            return retVal;
        }

        [HttpPost]
        [Route("api/ApiBook/UpdateBook")]
        public bool UpdateBook([FromBody] Book book)
        {
            if (string.IsNullOrEmpty(book.Token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[book.Token];
            if (personId < 0) return false;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                int count = DB.ExecuteScalar(con, "SELECT COUNT(BookId) FROM Book WHERE BookId = @bid",
                     new KeyValuePair<string, object>("bid", book.BookId));
                if (count < 1) return false;

                string[] queryParts = new string[]
                {
                    book.BookDescription != null ? "BookDescription = @bdesc" : "",
                    book.AuthorId != null ? "AuthorId = @aid" : "",
                    book.PublisherId != null ? "PublisherId = @pid" : "",
                    book.BookTitle != null ? "BookTitle = @btitle" : "",
                    book.PublishingDate != null ? "PublishingDate = @pubDate" : "",
                    book.Quantity != null ? "Quantity = @quantity" : ""
                };

                string queryBuilder = "";
                bool flag = true;
                // queryBuilder = "...SET A = @a, B = @b, ..."
                foreach (var str in queryParts)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (flag)
                        {
                            queryBuilder += str;
                            flag = false;
                        }
                        else
                        {
                            queryBuilder += ", " + str;
                        }
                    }
                }

                if (string.IsNullOrEmpty(queryBuilder)) return false;

                DB.ExecuteNonQuery(con, "UPDATE Book SET " + queryBuilder + " WHERE BookId = @bid " +
                    "--@aid --@pid --@quantity --@bdesc --@btitle --@pubDate",
                    new KeyValuePair<string, object>("bid", book.BookId),
                    new KeyValuePair<string, object>("aid", NegativeDBNull(book.AuthorId)),
                    new KeyValuePair<string, object>("pid", NegativeDBNull(book.PublisherId)),
                    new KeyValuePair<string, object>("quantity", book.Quantity ?? -1),
                    new KeyValuePair<string, object>("bdesc", book.BookDescription ?? ""),
                    new KeyValuePair<string, object>("btitle", book.BookTitle ?? ""),
                    new KeyValuePair<string, object>("pubDate", book.PublishingDate ?? ""));
            }
            return true;
        }

        public static object NegativeDBNull(int? val)
        {
            if (val == null) return "--any";
            if (val < 0) return DBNull.Value;
            else return val;
        }

        [HttpGet]
        [Route("api/ApiBook/LibrarianSearch")]
        public SearchResult LibrarianSearch(int page, int pageSize,
            string key = null)
        {
            SearchResult retVal = new SearchResult { Results = new List<Book>() };

            if (key == null) key = "";
            int start = (page - 1) * pageSize + 1;
            int end = start + pageSize - 1;

            List<KeyValuePair<string, object>> @params = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("key", key),
                new KeyValuePair<string, object>("start", start),
                new KeyValuePair<string, object>("end", end)
            };
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                string searchQuery = string.Format("SELECT * FROM ( " +
                    "SELECT ROW_NUMBER() OVER(ORDER BY BookId) AS[Row], Book.*, AuthorName, PublisherName FROM Book " +
                    "LEFT OUTER JOIN Author ON Author.AuthorId = Book.AuthorId " +
                    "LEFT OUTER JOIN Publisher ON Publisher.PublisherId = Book.PublisherId " +
                    "WHERE(BookTitle LIKE CONCAT('%', @key, '%')) " +
                    ") AS Result WHERE[Row] BETWEEN @start AND @end ");

                using (SqlDataReader reader = DB.ExecuteQuery(con, searchQuery, @params[0], @params[1], @params[2]))
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

                string countQuery = string.Format("SELECT COUNT(*) FROM Book WHERE (BookTitle LIKE " +
                "CONCAT('%',@key,'%'))");

                retVal.TotalCount = DB.ExecuteScalar(con, countQuery, @params[0], @params[1], @params[2]);
            }
            return retVal;
        }

        [HttpPost]
        [Route("api/ApiBook/Delete")]
        public bool Delete(Book book)
        {
            // making sure someone who has permission is doing this
            string token = book.Token;
            if (string.IsNullOrEmpty(token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return false;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                string imagePath;
                var bidparam = new KeyValuePair<string, object>("bid", book.BookId);
                using (var reader = DB.ExecuteQuery(con, "SELECT * FROM Book WHERE BookId = @bid", bidparam))
                {
                    if (reader.Read())
                    {
                        imagePath = reader["ThumbnailImage"]?.ToString();
                    }
                    else
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(imagePath))
                {
                    // delete the thumbnail image
                    var fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(imagePath));
                    fileInfo.Delete();
                }

                // any foreign key references should be also deleted
                DB.ExecuteNonQuery(con, "DELETE FROM Reservation WHERE BookId = @bid", bidparam);
                DB.ExecuteNonQuery(con, "DELETE FROM Book WHERE BookId = @bid", bidparam);
            }
            return true;
        }

        [HttpPost]
        [Route("api/ApiBook/UpdateImage")]
        public bool UpdateImage([FromUri]string token, [FromUri] int bookId)
        {
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return false;

            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            string extension = Path.GetExtension(file.FileName);
            if (extension.Equals(".png") || extension.Equals(".jpg") || extension.Equals(".jpeg"))
            {
                using (SqlConnection con = new SqlConnection(DB.ConnectionString))
                {
                    con.Open();
                    using (var reader = DB.ExecuteQuery(con, "SELECT ThumbnailImage FROM Book WHERE BookId = @id",
                    new KeyValuePair<string, object>("id", bookId)))
                    {
                        if (reader.Read() && !string.IsNullOrEmpty(reader["ThumbnailImage"]?.ToString()))
                        {
                            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(reader["ThumbnailImage"].ToString()));
                            fileInfo.Delete();
                        }
                    }

                    string newFileName = "/Media/" + TokenGenerator() + extension;
                    string targetPath = HttpContext.Current.Server.MapPath(newFileName);
                    file.SaveAs(targetPath);

                    DB.ExecuteNonQuery(con, "UPDATE Book SET ThumbnailImage = @img WHERE BookId = @id",
                        new KeyValuePair<string, object>("img", newFileName), new KeyValuePair<string, object>("id", bookId));
                }

                return true;
            }
            return false;
        }

        private string TokenGenerator()
        {
            return ApiAccountController.TokenGenerator();
        }
    }

    [DataContract]
    public class RentRequest
    {
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string PickupDateStr { get; set; }
        [DataMember]
        public int BookId { get; set; }
    }
}
