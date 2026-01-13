namespace PetSociety.API.DTOs.Events
{
    public class ActivityCalenderDto
    {
        public int ActivityId { get; set; }
        public string Title { get; set; } 

        public DateTime StartTime { get; set; }

        public bool IsRegistered { get; set; }
    }
}
