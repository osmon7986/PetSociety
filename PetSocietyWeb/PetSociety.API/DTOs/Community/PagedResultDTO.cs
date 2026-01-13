namespace PetSociety.API.DTOs.Community
{
    // 分頁DTO
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
