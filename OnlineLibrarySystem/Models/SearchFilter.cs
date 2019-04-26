namespace OnlineLibrarySystem.Models
{
    public class SearchFilter : CommonModel
    {
        public string Key { get; set; }
        public string SearchBy { get; set; }
        public int MinPub { get; set; }
        public int MaxPub { get; set; }
        public bool Match { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortType { get; set; }
        public string SortBy { get; set; }
    }
}