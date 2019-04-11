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
        public string BookDescription { get; set; }
        [DataMember]
        public string ThumbnailImage { get; set; }
        [DataMember]
        public DateTime PublishingDate { get; set; }
    }
}