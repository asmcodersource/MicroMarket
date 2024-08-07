namespace MicroMarket.Services.Ordering.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<OrderState> StateHistory { get; set; } = new List<OrderState>();
        public bool IsClosed { get; set; } = false;
    }
}
