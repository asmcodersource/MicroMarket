namespace MicroMarket.Services.Ordering.Models
{
    public class OrderState
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        string Name { get; set; } = string.Empty;
        string Description { get; set; } = string.Empty;
        public DateTime OccuredAt { get; init; } = DateTime.UtcNow;
    }
}
