using MicroMarket.Services.Ordering.Enums;
using MicroMarket.Services.Ordering.Models;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Ordering.Dtos
{
    public class DraftOrderUpdateDto
    {
        [StringLength(500, ErrorMessage = "Customer note can't be longer than 500 characters.")]
        public string CustomerNote { get; set; } = string.Empty;
        public DeliveryAddress DeliveryAddress { get; set; } = new DeliveryAddress();
        public PaymentType PaymentType { get; set; } = PaymentType.CreditCard;
    }
}
