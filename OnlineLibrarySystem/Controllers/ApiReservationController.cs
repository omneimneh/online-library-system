using OnlineLibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;

namespace OnlineLibrarySystem.Controllers
{
    public class ApiReservationController : ApiController
    {
        [HttpGet]
        [Route("api/ApiReservation/ReservationSearch")]
        public List<Reseravtion> ReservationSearch(string token, bool today, string key = "")
        {
            if (string.IsNullOrEmpty(token)) return null;
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return null;
            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return null;

            if (key == null) key = "";

            List<Reseravtion> reseravtions = new List<Reseravtion>();
            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                using (var reader = DB.ExecuteQuery(con, string.Format("SELECT Reservation.*, PersonInfo.Username, Book.BookTitle, " +
                    "(CASE WHEN IsDone = 1 THEN 0 WHEN IsPickedUp = 1 AND GETDATE() > ReturnDate THEN 3 WHEN IsPickedUp = 1 THEN 2 ELSE 1 END) AS Status " +
                    "FROM Reservation JOIN PersonInfo ON PersonInfo.PersonId = Reservation.PersonId " +
                    "JOIN Book ON Reservation.BookId = Book.BookId WHERE IsDone = 0 AND (Username LIKE CONCAT('%', @key ,'%')" +
                    "OR BookTitle LIKE CONCAT('%', @key, '%')) AND (CAST(PickupDate AS DATE) = CAST(GETDATE() AS DATE) OR CAST(ReturnDate AS DATE) = CAST(GETDATE() AS DATE) OR {0})" +
                    "ORDER BY PickupDate, OrderDate", today ? "1=0" : "1=1"), new KeyValuePair<string, object>("key", key)))
                {
                    while (reader.Read())
                    {
                        reseravtions.Add(new Reseravtion
                        {
                            ReservationId = Convert.ToInt32(reader["ReservationId"]),
                            PersonId = Convert.ToInt32(reader["PersonId"]),
                            BookId = Convert.ToInt32(reader["BookId"]),
                            PickupDate = Convert.ToDateTime(reader["PickupDate"]).ToString("MM/dd/yyyy"),
                            ReturnDate = Convert.ToDateTime(reader["ReturnDate"]).ToString("MM/dd/yyyy"),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            IsPickedUp = Convert.ToBoolean(reader["IsPickedUp"]),
                            IsDone = Convert.ToBoolean(reader["IsDone"]),
                            Username = reader["Username"].ToString(),
                            BookTitle = reader["BookTitle"]?.ToString(),
                            Status = (ReservationType)Convert.ToInt32(reader["Status"])
                        });
                    }
                }
            }
            return reseravtions;
        }

        [HttpPost]
        [Route("api/ApiReservation/Checkout")]
        public bool Checkout(Reseravtion reseravtion)
        {
            string token = reseravtion.Token;
            if (string.IsNullOrEmpty(token)) return false;
            int personId = TokenManager.TokenDictionaryHolder[token];
            if (personId < 0) return false;
            var person = new ApiAccountController().GetPerson(personId);
            if (person.PersonType < PersonType.Librarian) return false;

            using (var con = new SqlConnection(DB.ConnectionString))
            {
                con.Open();
                // If Is already picked up, make it done, else make it pickedUp
                DB.ExecuteNonQuery(con, "UPDATE Reservation SET IsDone = (CASE WHEN IsPickedUp = 0 THEN 0 ELSE 1 END), IsPickedUp = 1 " +
                    "WHERE ReservationId = @rid",
                    new KeyValuePair<string, object>("rid", reseravtion.ReservationId));
            }
            return true;
        }
    }
}