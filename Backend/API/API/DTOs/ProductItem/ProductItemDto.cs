namespace API.Server.DTOs.ProductItemDTO
{
    public class ProductItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
       
        public decimal Rating { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}