using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Basket.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid CatalogProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
