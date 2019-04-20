using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using OnlineLibrarySystem.Models;

namespace OnlineLibrarySystem.Controllers
{
    public class ApiAccountController : ApiController
    {
        [HttpPost]
        [Route("api/AccountApi/Login")]
        public string Login(string username, string password)
        {
            string encrypted = OneWayEncrpyt(password);

            string token = null;
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (SqlDataReader reader = DB.ExecuteQuery(con, "SELECT PersonId FROM PersonInfo WHERE Username = @user AND UserPassword = @pass",
                    new KeyValuePair<string, object>("user", username), new KeyValuePair<string, object>("pass", encrypted)))
                {

                    bool recordFound = reader.Read(); // contains at least one record
                    if (recordFound)
                    {
                        token = TokenManager.TokenDictionaryHolder.AddToken(TokenGenerator(), (int)reader["PersonId"]);
                    }
                }
            }
            return token;
        }

        public static string OneWayEncrpyt(string toEncrypt)
        {
            byte[] data = Encoding.ASCII.GetBytes("password");
            data = new MD5CryptoServiceProvider().ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        public static string TokenGenerator()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[20];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        [HttpPost]
        [Route("api/AccountApi/Signup")]
        public Person Signup(string username, string password, string verifyPassword)
        {
            Person retVal = null;
            if (username.Length < 4 || password.Length < 6 || !password.Equals(verifyPassword))
            {
                retVal = new Person { Error = true };
            }
            else
            {
                bool flag = true;
                using (SqlConnection con = new SqlConnection(DB.ConnectionString))
                {
                    con.Open();
                    using (SqlDataReader reader = DB.ExecuteQuery(con, "Select PersonId FROM PersonInfo WHERE username = @user",
                    new KeyValuePair<string, object>("user", username)))
                    {
                        if (reader.Read())
                        {
                            retVal = new Person { Error = true };
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        string encrypted = OneWayEncrpyt(password);
                        int personId = DB.ExecuteInsertQuery(con, "INSERT INTO Person(Username, UserPassword) VALUES (@user, @pass)",
                            new KeyValuePair<string, object>("user", username), new KeyValuePair<string, object>("pass", encrypted));

                        DB.ExecuteNonQuery(con, "INSERT INTO Student(PersonId) VALUES (@id)", new KeyValuePair<string, object>("id", personId));

                        using (SqlDataReader reader = DB.ExecuteQuery(con, "SELECT * FROM PersonInfo, Student " +
                            "WHERE Student.PersonId = PersonInfo.PersonId AND PersonInfo.PersonId = @id",
                            new KeyValuePair<string, object>("id", personId)))
                        {
                            if (reader.Read())
                            {
                                string token = TokenGenerator();
                                TokenManager.TokenDictionaryHolder.AddToken(token, personId);
                                retVal = new Person
                                {
                                    Username = reader["Username"].ToString(),
                                    Token = token
                                };
                            }
                        }
                    }
                }
            }
            return retVal;

        }

        [HttpGet]
        [Route("api/AccountApi/GetPerson")]
        public Person GetPerson(int personId)
        {
            Person retVal = null;
            if (personId < 0)
            {
                retVal = new Person { Error = true };
            }
            else
            {
                using (SqlConnection con = new SqlConnection(DB.ConnectionString))
                {
                    con.Open();
                    using (SqlDataReader reader = DB.ExecuteQuery(con, "SELECT * FROM PersonInfo WHERE PersonId = @id",
                    new KeyValuePair<string, object>("id", personId)))
                    {
                        if (reader.Read())
                        {
                            retVal = new Person
                            {
                                PersonId = personId,
                                Username = reader["Username"].ToString(),
                                PersonType = (PersonType)Convert.ToUInt32(reader["PersonType"]),
                                Email = reader["Email"]?.ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                ProfileImage = reader["ProfileImage"]?.ToString()
                            };
                        }
                        else
                        {
                            retVal = new Person { Error = true };
                        }
                    }
                }
            }
            return retVal;
        }

        [HttpGet]
        [Route("api/AccountApi/GetPersonOrders")]
        public List<Order> GetPersonOrders(string token, int count)
        {
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return null;

            List<Order> retVal = new List<Order>();
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (SqlDataReader reader = DB.ExecuteQuery(con, "SELECT TOP(@count) *, " +
                "(CASE WHEN IsDone = 1 THEN 0 WHEN GETDATE() < PickupDate  THEN 1 " +
                "WHEN GETDATE() < ReturnDate THEN 2 ELSE 3 END) AS OrderType FROM Reservation " +
                "JOIN Book ON Book.BookId = Reservation.BookId WHERE PersonId = @id ORDER BY OrderDate DESC",
                new KeyValuePair<string, object>("id", personId),
                new KeyValuePair<string, object>("count", count)))
                {

                    while (reader.Read())
                    {
                        retVal.Add(new Order
                        {
                            BookTitle = reader["BookTitle"].ToString(),
                            IsDone = Convert.ToBoolean(reader["IsDone"]),
                            OrderType = (OrderType)Convert.ToInt32(reader["OrderType"]),
                            OrderDate = Convert.ToDateTime(reader["OrderDate"]).ToString("MM/dd/yyyy"),
                            PickupDate = Convert.ToDateTime(reader["PickupDate"]).ToString("MM/dd/yyyy"),
                            ReturnDate = Convert.ToDateTime(reader["ReturnDate"]).ToString("MM/dd/yyyy")
                        });
                    }
                }
            }
            return retVal;
        }

        [HttpPost]
        [Route("api/AccountApi/UpdateProfile")]
        public bool UpdateProfile([FromBody]Person personInfo)
        {
            if (string.IsNullOrEmpty(personInfo.Token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[personInfo.Token];
            if (personId < 0) return false;
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                DB.ExecuteNonQuery(con, "UPDATE Person SET Email = @email, Phone = @phone WHERE PersonId = @id",
                new KeyValuePair<string, object>("email", personInfo.Email),
                new KeyValuePair<string, object>("phone", personInfo.Phone),
                new KeyValuePair<string, object>("id", personId));
            }
            return true;
        }

        [HttpPost]
        [Route("api/AccountApi/SetProfilePicture")]
        public bool SetProfilePicture([FromUri]string token)
        {
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return false;

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            string extension = Path.GetExtension(file.FileName);
            if (extension.Equals(".png") || extension.Equals(".jpg") || extension.Equals(".jpeg"))
            {
                using (SqlConnection con = new SqlConnection(DB.ConnectionString))
                {
                    con.Open();
                    using (var reader = DB.ExecuteQuery(con, "SELECT ProfileImage FROM PersonInfo WHERE PersonId = @id",
                    new KeyValuePair<string, object>("id", personId)))
                    {
                        if (reader.Read() && reader["ProfileImage"] != null)
                        {
                            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(reader["ProfileImage"].ToString()));
                            fileInfo.Delete();
                        }
                    }

                    string newFileName = "/Media/" + TokenGenerator() + extension;
                    string targetPath = HttpContext.Current.Server.MapPath(newFileName);
                    file.SaveAs(targetPath);

                    DB.ExecuteNonQuery(con, "UPDATE Person SET ProfileImage = @img WHERE PersonId = @id",
                        new KeyValuePair<string, object>("img", newFileName), new KeyValuePair<string, object>("id", personId));
                }

                return true;
            }
            return false;
        }
    }

}
