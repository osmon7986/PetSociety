namespace PetSociety.API.DTOs.Events
{
    public class ActivityMapDto
    {
        public string Location { get; set; }
        public string Title { get; set; }
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
