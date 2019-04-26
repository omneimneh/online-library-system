using System.Runtime.Serialization;

namespace OnlineLibrarySystem.Models
{
    [DataContract]
    public enum ReservationType
    {
        Done = 0,
        Ready = 1,
        Rented = 2,
        Late = 3
    }
}