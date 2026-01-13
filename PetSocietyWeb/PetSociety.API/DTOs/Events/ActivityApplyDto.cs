namespace PetSociety.API.DTOs.Events
{
    public class ActivityApplyDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Location { get; set; }

        public int MaxCapacity { get; set; }

        public DateTime RegistrationStartTime { get; set; }

        public DateTime RegistrationEndTime { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public int MemberId { get; set; }

        public string OrganizerName { get; set; }



    }
}
