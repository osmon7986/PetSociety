namespace PetSociety.API.DTOs.Events
{
    public class ActivityGuideDto
    {
                   
        public int GuideId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityEditorHtml { get; set; }
    }
}