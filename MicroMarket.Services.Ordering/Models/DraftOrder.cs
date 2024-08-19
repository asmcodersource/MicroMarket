using MicroMarket.Services.Ordering.Enums;

namespace MicroMarket.Services.Ordering.Models
{
    public class DraftOrder
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerNote { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DeliveryAddress DeliveryAddress { get; set; } = new DeliveryAddress();
        public PaymentType PaymentType { get; set; } = PaymentType.CreditCard;
        public ICollection<Item> ClaimedItems { get; set; } = new List<Item>();
    }
}
