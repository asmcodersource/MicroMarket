using MicroMarket.Services.Basket.Models;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Basket.Dtos
{
    public class BasketItemGetDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Guid CatalogProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public bool IsProductDeleted { get; set; }
        public bool IsProductActive { get; set; }
        public bool IsEnough { get; init; }

        public BasketItemGetDto(Item item) 
        {
            Id = item.Id;
            CustomerId = item.CustomerId;
            Quantity = item.Quantity;
            ProductId = item.Product.Id;
            CatalogProductId = item.Product.CatalogProductId;
            ProductName = item.Product.Name;
            ProductPrice = item.Product.Price;
            IsProductDeleted = item.Product.IsDeleted;
            IsProductActive = item.Product.IsActive;
            IsEnough = item.Quantity <= item.Product.Quantity;
        }
    }
}
