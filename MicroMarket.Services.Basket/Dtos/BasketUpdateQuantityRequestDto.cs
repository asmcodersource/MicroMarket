using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Basket.Dtos
{
    public class BasketUpdateQuantityRequestDto
    {
        [Range(1, 1000, ErrorMessage = "The number of products in the basket must be greater than 0 and less than 1000.")]
        public int NewQuantity { get; set; }
    }
}
