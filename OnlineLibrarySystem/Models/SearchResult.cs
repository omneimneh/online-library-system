using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public List<Book> Results { get; set; }
        [DataMember]
        public int TotalCount { get; set; }
    }
}