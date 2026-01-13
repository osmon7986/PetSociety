using PetSociety.API.Models;

namespace PetSociety.API.DTOs.Events
{
    public class GetActivityCommentDto
    {

        public int ActivityId { get; set; }

        public string ActivityComment1 { get; set; }

        public DateTime CreateDate { get; set; }

        public int MemberId { get; set; }

        public virtual Member Member { get; set; }

        public string MemberName { get; set; }
    }
}
