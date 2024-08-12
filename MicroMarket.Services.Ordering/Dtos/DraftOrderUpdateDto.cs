using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Dtos
{
    public class DraftOrderUpdateDto
    {
        public DeliveryAddress DeliveryAddress { get; set; } = new DeliveryAddress();
        public PaymentType PaymentType { get; set; } = PaymentType.CreditCard;
    }
}
