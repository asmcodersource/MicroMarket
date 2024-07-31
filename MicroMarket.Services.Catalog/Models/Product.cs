namespace MicroMarket.Services.Catalog.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId {  get; set; }
        public Category? Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? Brand { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
