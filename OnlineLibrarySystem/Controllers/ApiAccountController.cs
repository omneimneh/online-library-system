using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

            using (SqlDataReader reader = DB.ExecuteQuery("SELECT PersonId FROM Person WHERE Username = @user AND UserPassword = @pass",
                new KeyValuePair<string, object>("user", username), new KeyValuePair<string, object>("pass", encrypted)))
            {

                bool recordFound = reader.Read(); // contains at least one record
                string token = null;
                if (recordFound)
                {
                    token = TokenManager.TokenDictionaryHolder.AddToken(TokenGenerator(), (int)reader["PersonId"]);
                }
                reader.Close();
                return token;
            }
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
            if (username.Length < 4 || password.Length < 6 || !password.Equals(verifyPassword))
            {
                return new Person { Error = true };
            }

            using (SqlDataReader reader = DB.ExecuteQuery("Select PersonId FROM Person WHERE username = @user",
                new KeyValuePair<string, object>("user", username)))
            {
                if (reader.Read())
                {
                    return new Person { Error = true };
                }
            }

            string encrypted = OneWayEncrpyt(password);
            int personId = DB.ExecuteInsertQuery("INSERT INTO Person(Username, UserPassword) VALUES (@user, @pass)",
                new KeyValuePair<string, object>("user", username), new KeyValuePair<string, object>("pass", encrypted));

            DB.ExecuteNonQuery("INSERT INTO Student(PersonId) VALUES (@id)", new KeyValuePair<string, object>("id", personId));

            using (SqlDataReader reader = DB.ExecuteQuery("SELECT * FROM Person, Student " +
                "WHERE Student.PersonId = Person.PersonId AND Person.PersonId = @id",
                new KeyValuePair<string, object>("id", personId)))
            {
                if (reader.Read())
                {
                    string token = TokenGenerator();
                    TokenManager.TokenDictionaryHolder.AddToken(token, personId);
                    return new Person
                    {
                        Username = reader["Username"].ToString(),
                        Token = token
                    };
                }
            }

            return new Person { Error = true };

        }

        [HttpGet]
        [Route("api/AccountApi/GetPerson")]
        public Person GetPerson(int personId)
        {
            if (personId < 0) return new Person { Error = true };

            using (SqlDataReader reader = DB.ExecuteQuery("SELECT *, CASE " +
                "WHEN EXISTS(SELECT PersonId FROM Maintainer WHERE PersonId = @id) THEN 3 " +
                "WHEN EXISTS(SELECT PersonId FROM Librarian WHERE PersonId = @id) THEN 2 " +
                "WHEN EXISTS(SELECT PersonId FROM Professor WHERE PersonId = @id) THEN 1 " +
                "WHEN EXISTS(SELECT PersonId FROM Student WHERE PersonId = @id) THEN 0 " +
                "END AS PersonType FROM Person WHERE PersonId = @id", new KeyValuePair<string, object>("id", personId)))
            {
                if (reader.Read())
                {
                    return new Person
                    {
                        PersonId = personId,
                        Username = reader["Username"].ToString(),
                        PersonType = (PersonType)Convert.ToUInt32(reader["PersonType"])
                    };
                }
                return new Person { Error = true };
            }
        }

        [HttpGet]
        [Route("api/AccountApi/GetPersonOrders")]
        public List<Order> GetPersonOrders(string token, int count)
        {
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return null;

            using (SqlDataReader reader = DB.ExecuteQuery("SELECT TOP(@count) *, " +
                "(CASE WHEN IsDone = 1 THEN 0 WHEN GETDATE() < PickupDate  THEN 1 " +
                "WHEN GETDATE() < ReturnDate THEN 2 ELSE 3 END) AS OrderType FROM Reservation " +
                "JOIN Book ON Book.BookId = Reservation.BookId WHERE PersonId = @id ORDER BY OrderDate",
                new KeyValuePair<string, object>("id", personId),
                new KeyValuePair<string, object>("count", count)))
            {
                List<Order> retVal = new List<Order>();

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
                return retVal;
            }
        }
    }

}
