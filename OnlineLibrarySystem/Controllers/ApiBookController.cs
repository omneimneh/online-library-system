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
                            BookId = ConvertToNullableInt32(reader["BookId"]),
                            BookTitle = reader["BookTitle"].ToString(),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            AuthorName = reader["AuthorName"]?.ToString(),
                            PublishingDate = Convert.ToDateTime(reader["PublishingDate"]).ToString("MM/dd/yyyy"),
                            ThumbnailImage = reader["ThumbnailImage"]?.ToString(),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            Quantity = ConvertToNullableInt32(reader["Quantity"]),
                            NextAvailable = ExtractDate(reader["NextAvailable"]),
                            Price = Convert.ToDecimal(reader["Price"])
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
             string sortBy, string sortType, string key = null, string match = "off")
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
                string searchQuery = string.Format("SELECT * FROM ( SELECT ROW_NUMBER() OVER(ORDER BY {4} {5}) AS[Row], " +
                "BookInfo.* FROM BookInfo LEFT OUTER JOIN(Select COUNT(BookId) AS[Count], BookId FROM Reservation GROUP " +
                "BY BookId) AS Res ON BookInfo.BookId = Res.BookId WHERE (BookTitle LIKE CONCAT('{3}',@key,'{3}') OR {0}) " +
                "AND(AuthorName LIKE CONCAT('{3}',@key,'{3}') OR {1}) AND (PublisherName LIKE CONCAT('{3}',@key,'{3}') OR " +
                "{2}) AND (PublishingDate IS NULL OR YEAR(PublishingDate) BETWEEN @min AND @max)) AS Result WHERE[Row] BETWEEN @start AND @end",
                byBook, byAuthor, byPublisher, match.Equals("on") ? "" : "%", ExtractSortBy(sortBy), ExtractSortType(sortType));

                using (SqlDataReader reader = DB.ExecuteQuery(con, searchQuery, @params[0], @params[1], @params[2], @params[3], @params[4]))
                {
                    while (reader.Read())
                    {
                        retVal.Results.Add(new Book
                        {
                            BookId = ConvertToNullableInt32(reader["BookId"]),
                            AuthorName = reader["AuthorName"]?.ToString(),
                            BookTitle = reader["BookTitle"].ToString(),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            PublishingDate = Convert.ToDateTime(reader["PublishingDate"]).ToString("MM/dd/yyyy"),
                            ThumbnailImage = reader["ThumbnailImage"].ToString(),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            Quantity = ConvertToNullableInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"])
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

        private string ExtractSortType(string sortType)
        {
            return sortType.ToLower().Equals("asc") ? "ASC" : "DESC";
        }

        private string ExtractSortBy(string sortBy)
        {
            switch (sortBy)
            {
                case "book":
                    return "BookTitle";
                case "author":
                    return "AuthorName";
                case "publisher":
                    return "PublisherName";
                case "date":
                    return "DateAdded";
                default:
                    return "[Count]";
            }
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
                            BookId = ConvertToNullableInt32(reader["BookId"]),
                            AuthorName = reader["AuthorName"]?.ToString(),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            BookTitle = reader["BookTitle"].ToString(),
                            PublishingDate = ExtractDate(reader["PublishingDate"]),
                            ThumbnailImage = reader["ThumbnailImage"].ToString(),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            Quantity = ConvertToNullableInt32(reader["Quantity"]),
                            NextAvailable = ExtractDate(reader["NextAvailable"]),
                            Price = Convert.ToDecimal(reader["Price"])
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
                        if (ConvertToNullableInt32(reader["Quantity"]) < 1) return false;
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
                            Quantity = ConvertToNullableInt32(reader["Quantity"]),
                            BookId = ConvertToNullableInt32(reader["BookId"]),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            PublishingDate = ExtractDate(reader["PublishingDate"]),
                            Price = Convert.ToDecimal(reader["Price"])
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
                            AuthorId = ConvertToNullableInt32(reader["AuthorId"]),
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
                            PublisherId = ConvertToNullableInt32(reader["PublisherId"]),
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
                    book.Quantity != null ? "Quantity = @quantity" : "",
                    book.Price != null ? "Price = @price" : ""
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
                    "--@aid --@pid --@quantity --@bdesc --@btitle --@pubDate --@price",
                    new KeyValuePair<string, object>("bid", book.BookId),
                    new KeyValuePair<string, object>("aid", PositiveOrDBNull(book.AuthorId)),
                    new KeyValuePair<string, object>("pid", PositiveOrDBNull(book.PublisherId)),
                    new KeyValuePair<string, object>("quantity", book.Quantity ?? -1),
                    new KeyValuePair<string, object>("bdesc", book.BookDescription ?? ""),
                    new KeyValuePair<string, object>("btitle", book.BookTitle ?? ""),
                    new KeyValuePair<string, object>("price", PositiveOrDBNull(book.Price)),
                    new KeyValuePair<string, object>("pubDate", book.PublishingDate ?? ""));
            }
            return true;
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
                    "SELECT ROW_NUMBER() OVER(ORDER BY DateAdded DESC) AS[Row], Book.*, AuthorName, PublisherName FROM Book " +
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
                            BookId = ConvertToNullableInt32(reader["BookId"]),
                            AuthorId = ConvertToNullableInt32(reader["AuthorId"]),
                            AuthorName = reader["AuthorName"]?.ToString(),
                            BookTitle = reader["BookTitle"].ToString(),
                            BookDescription = reader["BookDescription"]?.ToString(),
                            PublishingDate = Convert.ToDateTime(reader["PublishingDate"]).ToString("MM/dd/yyyy"),
                            ThumbnailImage = reader["ThumbnailImage"].ToString(),
                            PublisherId = ConvertToNullableInt32(reader["PublisherId"]),
                            PublisherName = reader["PublisherName"]?.ToString(),
                            Quantity = ConvertToNullableInt32(reader["Quantity"]),
                            Price = Convert.ToDecimal(reader["Price"])
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
            return UpdateImageImpl(file, bookId);
        }

        private bool UpdateImageImpl(HttpPostedFile file, int bookId)
        {
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

        [HttpPost]
        [Route("api/ApiBook/AddBook")]
        public bool AddBook([FromUri] Book book)
        {
            if (string.IsNullOrEmpty(book.Token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[book.Token];
            if (personId < 0) return false;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            if (string.IsNullOrEmpty(book.BookTitle) || string.IsNullOrEmpty(book.PublishingDate)) return false;

            int bookId;
            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                bookId = DB.ExecuteInsertQuery(con, "INSERT INTO Book (BookTitle, BookDescription, AuthorId, PublisherId, Price, " +
                    "PublishingDate) VALUES (@btitle, @bdesc, @aid, @pid, @price, @pubDate)",
                    new KeyValuePair<string, object>("btitle", book.BookTitle),
                    new KeyValuePair<string, object>("bdesc", StringOrDBNull(book.BookDescription)),
                    new KeyValuePair<string, object>("aid", PositiveOrDBNull(book.AuthorId)),
                    new KeyValuePair<string, object>("pid", PositiveOrDBNull(book.PublisherId)),
                    new KeyValuePair<string, object>("price", book.Price),
                    new KeyValuePair<string, object>("pubDate", StringOrDBNull(book.PublishingDate)));
            }

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                UpdateImageImpl(file, bookId);
            }

            return true;
        }

        [HttpPost]
        [Route("api/ApiBook/AddPublisher")]
        public int AddPublisher(Publisher publisher)
        {
            if (string.IsNullOrEmpty(publisher.Token)) return -1;
            int personId = TokenManager.TokenDictionaryHolder[publisher.Token];
            if (personId < 0) return -1;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return -1;

            if (string.IsNullOrEmpty(publisher.PublisherName)) return -1;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                int pubId = DB.ExecuteInsertQuery(con, "INSERT INTO Publisher (PublisherName) VALUES (@pubname)",
                    new KeyValuePair<string, object>("pubname", publisher.PublisherName));
                return pubId;
            }

        }

        [HttpPost]
        [Route("api/ApiBook/AddAuthor")]
        public int AddAuthor(Author author)
        {
            if (string.IsNullOrEmpty(author.Token)) return -1;
            int personId = TokenManager.TokenDictionaryHolder[author.Token];
            if (personId < 0) return -1;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return -1;

            if (string.IsNullOrEmpty(author.AuthorName)) return -1;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                int auId = DB.ExecuteInsertQuery(con, "INSERT INTO Author (AuthorName) VALUES (@auname)",
                    new KeyValuePair<string, object>("auname", author.AuthorName));
                return auId;
            }
        }

        [HttpPost]
        [Route("api/ApiBook/DeleteAuthor")]
        public bool DeleteAuthor(Author author)
        {
            if (string.IsNullOrEmpty(author.Token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[author.Token];
            if (personId < 0) return false;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                DB.ExecuteNonQuery(con, "UPDATE Book SET AuthorId = NULL WHERE AuthorId = @aid",
                    new KeyValuePair<string, object>("aid", author.AuthorId));
                DB.ExecuteNonQuery(con, "DELETE FROM Author WHERE AuthorId = @aid",
                    new KeyValuePair<string, object>("aid", author.AuthorId));
            }
            return true;
        }

        [HttpPost]
        [Route("api/ApiBook/DeletePublisher")]
        public bool DeletePublisher(Publisher publisher)
        {
            if (string.IsNullOrEmpty(publisher.Token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[publisher.Token];
            if (personId < 0) return false;
            Person person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                DB.ExecuteNonQuery(con, "UPDATE Book SET PublisherId = NULL WHERE PublisherId = @pid",
                    new KeyValuePair<string, object>("pid", publisher.PublisherId));
                DB.ExecuteNonQuery(con, "DELETE FROM Publisher WHERE PublisherId = @pid",
                    new KeyValuePair<string, object>("pid", publisher.PublisherId));
            }
            return true;
        }

        private static int ConvertToNullableInt32(object obj)
        {
            if (obj == null) return -1;
            if (obj == DBNull.Value) return -1;
            return Convert.ToInt32(obj);
        }

        private object StringOrDBNull(string val)
        {
            if (val == null) return DBNull.Value;
            return val;
        }

        public static object PositiveOrDBNull(int? val)
        {
            if (val == null) return "--any";
            if (val < 0) return DBNull.Value;
            else return val;
        }

        public static object PositiveOrDBNull(decimal? val)
        {
            if (val == null) return "--any";
            if (val < 0) return DBNull.Value;
            else return val;
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
