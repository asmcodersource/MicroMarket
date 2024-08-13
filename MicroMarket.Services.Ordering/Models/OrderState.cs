namespace MicroMarket.Services.Ordering.Models
{
    public class OrderState
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public OrderStateType Type { get; set; } = OrderStateType.Default;
        public DateTime OccuredAt { get; init; } = DateTime.UtcNow;
    }
}
