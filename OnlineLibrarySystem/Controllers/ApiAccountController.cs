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
            byte[] data = Encoding.UTF8.GetBytes(toEncrypt + "password");
            data = new SHA256CryptoServiceProvider().ComputeHash(data);
            return BitConverter.ToString(data);
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
        public List<Reseravtion> GetPersonOrders(string token, int count)
        {
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return null;

            List<Reseravtion> retVal = new List<Reseravtion>();
            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (SqlDataReader reader = DB.ExecuteQuery(con, "SELECT TOP(@count) Reservation.*, Book.BookTitle, Reservation.Quantity * Book.Price AS Price, " +
                "(CASE WHEN IsDone = 1 THEN 0 WHEN IsPickedUp = 1 AND GETDATE() > ReturnDate THEN 3 WHEN IsPickedUp = 1 THEN 2 ELSE 1 END) AS Status " +
                "FROM Reservation JOIN Book ON Book.BookId = Reservation.BookId WHERE PersonId = @id ORDER BY OrderDate DESC",
                new KeyValuePair<string, object>("id", personId),
                new KeyValuePair<string, object>("count", count)))
                {

                    while (reader.Read())
                    {
                        retVal.Add(new Reseravtion
                        {
                            BookTitle = reader["BookTitle"].ToString(),
                            IsDone = Convert.ToBoolean(reader["IsDone"]),
                            Status = (ReservationType)Convert.ToInt32(reader["Status"]),
                            OrderDate = Convert.ToDateTime(reader["OrderDate"]).ToString("MM/dd/yyyy"),
                            PickupDate = Convert.ToDateTime(reader["PickupDate"]).ToString("MM/dd/yyyy"),
                            ReturnDate = Convert.ToDateTime(reader["ReturnDate"]).ToString("MM/dd/yyyy"),
                            Price = Convert.ToDecimal(reader["Price"])
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
                        if (reader.Read() && !string.IsNullOrEmpty(reader["ProfileImage"]?.ToString()))
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

        [HttpPost]
        [Route("api/AccountApi/AddPerson")]
        public Person AddPerson([FromBody]Person librarian)
        {
            if (string.IsNullOrEmpty(librarian.Token)) return null;
            int personId = TokenManager.TokenDictionaryHolder[librarian.Token];
            if (personId < 0) return null;
            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Admin) return null;

            if (string.IsNullOrEmpty(librarian.Username) || string.IsNullOrEmpty(librarian.UserPassword)) return null;
            if (librarian.Username.Length < 4 || librarian.UserPassword.Length < 6) return null;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                int exists = DB.ExecuteScalar(con, "SELECT COUNT(*) FROM Person WHERE Username = @username",
                    new KeyValuePair<string, object>("username", librarian.Username));
                if (exists > 0) return null;

                if (librarian.PersonType == PersonType.Librarian)
                {
                    librarian.PersonId = DB.ExecuteInsertQuery(con, "INSERT INTO Person (Username , UserPassword) VALUES" +
                        " (@lname, @lpass) ",
                        new KeyValuePair<string, object>("lname", librarian.Username),
                        new KeyValuePair<string, object>("lpass", OneWayEncrpyt(librarian.UserPassword)));
                    DB.ExecuteNonQuery(con, "INSERT INTO Librarian (PersonId) VALUES (@pid)", new KeyValuePair<string, object>("pid", librarian.PersonId));

                }
                else if (librarian.PersonType == PersonType.Proffessor)
                {
                    librarian.PersonId = DB.ExecuteInsertQuery(con, "INSERT INTO Person (Username , UserPassword) VALUES" +
                        " (@pname, @Ppass) ",
                        new KeyValuePair<string, object>("pname", librarian.Username),
                        new KeyValuePair<string, object>("Ppass", OneWayEncrpyt(librarian.UserPassword)));
                    DB.ExecuteNonQuery(con, "INSERT INTO Professor (PersonId) VALUES (@pid)", new KeyValuePair<string, object>("pid", librarian.PersonId));

                }
                else if (librarian.PersonType == PersonType.Admin)
                {
                    librarian.PersonId = DB.ExecuteInsertQuery(con, "INSERT INTO Person (Username , UserPassword) VALUES" +
                        " (@pname, @Ppass) ",
                        new KeyValuePair<string, object>("pname", librarian.Username),
                        new KeyValuePair<string, object>("Ppass", OneWayEncrpyt(librarian.UserPassword)));
                    DB.ExecuteNonQuery(con, "INSERT INTO Librarian (PersonId) VALUES (@pid)", new KeyValuePair<string, object>("pid", librarian.PersonId));
                    DB.ExecuteNonQuery(con, "INSERT INTO Maintainer (PersonId) VALUES (@pid)", new KeyValuePair<string, object>("pid", librarian.PersonId));
                }
            }
            return librarian;
        }

        [HttpGet]
        [Route("api/AccountApi/GetLibrarians")]
        public List<Person> GetLibrarians(string token)
        {
            List<Person> retVal = new List<Person>();
            if (string.IsNullOrEmpty(token)) return null;
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return null;
            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Admin) return null;

            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (var reader = DB.ExecuteQuery(con, "SELECT * FROM Librarian JOIN PersonInfo ON PersonInfo.PersonId " +
                    "= Librarian.PersonId WHERE Librarian.PersonId != @pid", new KeyValuePair<string, object>("pid", personId)))
                {
                    while (reader.Read())
                    {
                        retVal.Add(new Person
                        {
                            PersonId = Convert.ToInt32(reader["PersonId"]),
                            Username = reader["Username"]?.ToString(),
                            PersonType = (PersonType)Convert.ToInt32(reader["PersonType"])
                        });

                    }
                }
            }
            return retVal;
        }

        [HttpGet]
        [Route("api/AccountApi/GetPersons")]
        public List<Person> GetPersons(string token)
        {
            List<Person> retVal = new List<Person>();
            if (string.IsNullOrEmpty(token)) return null;
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return null;
            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Admin) return null;

            using (SqlConnection con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (var reader = DB.ExecuteQuery(con, "SELECT * FROM PersonInfo " +
                    "WHERE PersonInfo.PersonId != @pid", new KeyValuePair<string, object>("pid", personId)))
                {
                    while (reader.Read())
                    {
                        retVal.Add(new Person
                        {
                            PersonId = Convert.ToInt32(reader["PersonId"]),
                            Username = reader["Username"]?.ToString(),
                            PersonType = (PersonType)Convert.ToInt32(reader["PersonType"])
                        });
                    }
                }
            }
            return retVal;
        }

        [HttpPost]
        [Route("api/AccountApi/DeletePerson")]
        public bool DeletePerson(Person user)
        {
            string token = user.Token;
            if (string.IsNullOrEmpty(token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return false;
            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Admin) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                var pidparam = new KeyValuePair<string, object>("pid", user.PersonId);

                int exists = DB.ExecuteScalar(con, "SELECT COUNT(*) FROM Maintainer WHERE PersonId = @pid", pidparam);
                if (exists > 0) return false;

                DB.ExecuteNonQuery(con, "UPDATE Person SET Deleted = 1 WHERE PersonId = @pid", pidparam);
            }
            return true;
        }

        [HttpPost]
        [Route("api/AccountApi/ChangePassword")]
        public bool ChangePassword(ChangePasswordData passwordData)
        {
            string Token = passwordData.Token,
                oldPassword = passwordData.OldPassword,
                newPassword = passwordData.NewPassword,
                newPasswordVerify = passwordData.NewPasswordVerify;

            if (string.IsNullOrEmpty(Token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[Token];
            if (personId < 0) return false;

            if (newPassword == null || newPassword.Length < 6 || !newPassword.Equals(newPasswordVerify)) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                int passTrue = DB.ExecuteScalar(con, "SELECT COUNT(*) FROM Person WHERE PersonId = @pid AND UserPassword = @pass",
                    new KeyValuePair<string, object>("pid", personId),
                    new KeyValuePair<string, object>("pass", OneWayEncrpyt(oldPassword)));

                if (passTrue == 0) return false;

                DB.ExecuteQuery(con, "UPDATE Person SET UserPassword = @pass WHERE PersonId = @pid",
                    new KeyValuePair<string, object>("pid", personId),
                    new KeyValuePair<string, object>("pass", OneWayEncrpyt(newPassword)));
            }
            return true;
        }

    }

    public class ChangePasswordData
    {
        public string Token { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordVerify { get; set; }
    }
}
