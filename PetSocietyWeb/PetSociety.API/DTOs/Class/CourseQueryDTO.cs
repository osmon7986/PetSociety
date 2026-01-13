namespace PetSociety.API.DTOs.Class
{
    public class CourseQueryDTO
    {
        private int _pageSize = 8; // 預設一頁顯示筆數
        private const int MaxPageSize = 20; // 保留上限，防止被查詢過多資料

        public int PageIndex { get; set; } = 1;
        public int PageSize 
        {
            get => _pageSize; 
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; 
        }
        public string? Search { get; set; }
        public int? CategoryId { get; set; }

    }
}
