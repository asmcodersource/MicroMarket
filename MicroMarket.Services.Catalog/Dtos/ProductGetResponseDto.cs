using MicroMarket.Services.Catalog.Models;

namespace MicroMarket.Services.Catalog.Dtos
{
    public class ProductGetResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? Brand { get; set; }
        public string? Manufacturer { get; set; }
        public decimal? DiscountPrice { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        public ProductGetResponseDto(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            CategoryId = product.CategoryId;
            Price = product.Price;
            StockQuantity = product.StockQuantity;
            Weight = product.Weight;
            Dimensions = product.Dimensions;
            Brand = product.Brand;
            Manufacturer = product.Manufacturer;
            DiscountPrice = product.DiscountPrice;
            DiscountStartDate = product.DiscountStartDate;
            DiscountEndDate = product.DiscountEndDate;
        }
    }
}
