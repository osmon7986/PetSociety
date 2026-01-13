namespace PetSociety.API.DTOs.Class
{
    public class CoursePagedDTO
    {
        public List<CourseDTO> Items { get; set; } = new List<CourseDTO>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
