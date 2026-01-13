namespace PetSociety.API.DTOs.Community
{
    public class CommentDTO
    {
        public int CommentId { get; set; }

        public int ArticleId { get; set; }

        public int MemberId { get; set; }


        public string Content { get; set; }

        public DateTime PostDate { get; set; }


    }
}
