namespace PetSociety.API.DTOs.Events
{
    public class CreateActivityRequest
    {
        public ActivityApplyDto ApplyData { get; set; }
        public ActivityGuideDto GuideData { get; set; }
    }
}
