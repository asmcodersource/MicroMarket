using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Basket.Models
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public Product Product { get; set; } = null!;
    }
}
