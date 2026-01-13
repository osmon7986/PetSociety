namespace PetSociety.API.DTOs.Shop
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Thumbnail { get; set; }
        public string? Description { get; set; } 
        public List<string> Images { get; set; } = new List<string>();
    }
}
