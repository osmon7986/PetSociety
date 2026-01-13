namespace PetSocietyWeb.Areas.User.ViewModels
{
    public class NotificationVM
    {
        public int NotificationId { get; set; }
        public int MemberId { get; set; }
        public int? TypeId { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ReadTime { get; set; }

        //額外欄位：顯示格式用
        public bool IsRead => ReadTime != null;
        public string CreatedTimeString => CreateDate.ToString("yyyy-MM-dd HH:mm");
        public string ReadTimeString => ReadTime?.ToString("yyyy-MM-dd HH:mm");
    }
}
