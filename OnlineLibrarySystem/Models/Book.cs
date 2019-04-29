using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public class Book : CommonModel
    {
        [DataMember]
        public int BookId { get; set; }
        [DataMember]
        public string BookTitle { get; set; }
        [DataMember]
        public string AuthorName { get; set; }
        [DataMember]
        public string PublisherName { get; set; }
        [DataMember]
        public string BookDescription { get; set; }
        [DataMember]
        public string ThumbnailImage { get; set; }
        [DataMember]
        public string PublishingDate { get; set; }
        [DataMember]
        public int? AuthorId { get; set; }
        [DataMember]
        public int? PublisherId { get; set; }
        [DataMember]
        public int? Quantity { get; set; }
        [DataMember]
        public string NextAvailable { get; set; }
        [DataMember]
        public decimal? Price { get; set; }
    }
}