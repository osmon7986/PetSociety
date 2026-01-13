namespace PetSociety.API.DTOs.Events
{
    public class ActivityInfoDto
    {


        public DateTime StartTime { get; set; }

        public string Location { get; set; }

        public int MaxCapacity { get; set; }

        public DateTime RegistrationEndTime { get; set; }

        public string OrganizerName { get; set; }


    }
}
