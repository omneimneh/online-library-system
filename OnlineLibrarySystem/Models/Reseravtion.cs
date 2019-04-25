using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public class Reseravtion : CommonModel
    {
        [DataMember]
        public int ReservationId { get; set; }
        [DataMember]
        public int PersonId { get; set; }
        [DataMember]
        public int BookId { get; set; }
        [DataMember]
        public string PickupDate { get; set; }
        [DataMember]
        public string ReturnDate { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public bool IsPickedUp { get; set; }
        [DataMember]
        public bool IsDone { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string BookTitle { get; set; }
    }
}