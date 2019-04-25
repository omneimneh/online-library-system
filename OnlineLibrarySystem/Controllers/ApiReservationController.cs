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
        public List<Reseravtion> ReservationSearch(string token, string key = "")
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
                using (var reader = DB.ExecuteQuery(con, "SELECT Reservation.*, PersonInfo.Username, Book.BookTitle FROM Reservation " +
                    "JOIN PersonInfo ON PersonInfo.PersonId = Reservation.PersonId " +
                    "JOIN Book ON Reservation.BookId = Book.BookId WHERE IsDone = 0 AND (Username LIKE CONCAT('%', @key ,'%')" +
                    "OR BookTitle LIKE CONCAT('%', @key, '%'))" +
                    "ORDER BY PickupDate, OrderDate", new KeyValuePair<string, object>("key", key)))
                {
                    while (reader.Read())
                    {
                        var res = new Reseravtion
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
                            BookTitle = reader["BookTitle"]?.ToString()
                        };
                        if (!res.IsPickedUp) res.Status = "ready";
                        else if (DateTime.Now.CompareTo(Convert.ToDateTime(reader["ReturnDate"])) < 0) res.Status = "rented";
                        else res.Status = "late";
                        reseravtions.Add(res);
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