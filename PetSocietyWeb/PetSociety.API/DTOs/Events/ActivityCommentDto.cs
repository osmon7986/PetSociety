namespace PetSociety.API.DTOs.Events
{
    public class ActivityCommentDto
    {
        public int ActivityId { get; set; }

        public string ActivityComment1 { get; set; }

        public DateTime CreateDate { get; set; }

        public int MemberId { get; set; }
    }
}
