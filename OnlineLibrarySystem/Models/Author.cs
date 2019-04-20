using System.Runtime.Serialization;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public class Author
    {
        [DataMember]
        public int AuthorId { get; set; }
        [DataMember]
        public string AuthorName { get; set; }
    }
}