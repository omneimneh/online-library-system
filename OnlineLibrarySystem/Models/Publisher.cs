using System.Runtime.Serialization;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public class Publisher
    {
        [DataMember]
        public int PublisherId { get; set; }
        [DataMember]
        public string PublisherName { get; set; }
    }
}