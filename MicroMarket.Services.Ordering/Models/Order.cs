namespace MicroMarket.Services.Ordering.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerNote { get; set; } = string.Empty;
        public string ManagerNote { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public PaymentType PaymentType { get; set; } = PaymentType.CashOnDelivery;
        public DeliveryAddress DeliveryAddress { get; set; } = null!;
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<OrderState> StateHistory { get; set; } = new List<OrderState>();
        public bool IsDelivered { get; set; } = false;
        public bool IsPaid { get; set; } = false;
        public bool IsClosed { get; set; } = false;
    }
}
