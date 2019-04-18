using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public class Order : CommonModel
    {
        [DataMember]
        public string BookTitle { get; set; }
        [DataMember]
        public string OrderDate { get; set; }
        [DataMember]
        public string PickupDate { get; set; }
        [DataMember]
        public string  ReturnDate { get; set; }
        [DataMember]
        public bool IsDone { get; set; }
        [DataMember]
        public OrderType OrderType { get; set; }
    }
}