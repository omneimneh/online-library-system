using System.Runtime.Serialization;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public enum PersonType
    {
        Student = 0,
        Proffessor = 1,
        Librarian = 2,
        Admin = 3
    }
}