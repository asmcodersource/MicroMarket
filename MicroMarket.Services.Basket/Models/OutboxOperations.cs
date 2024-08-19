using MicroMarket.Services.Basket.Enums;

namespace MicroMarket.Services.Basket.Models
{
    public class OutboxOperations
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; } = Guid.Empty;
        public Guid AggregateId { get; set; }
        public OutboxState State { get; set; }
        public OutboxOperationType OperationType { get; set; }
        public DateTime CreateAt { get; init; } = DateTime.UtcNow;
        public DateTime ProcessedAt { get; init; }
    }
}
