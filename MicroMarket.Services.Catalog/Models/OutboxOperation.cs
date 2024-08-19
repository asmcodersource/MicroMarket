using MicroMarket.Services.Catalog.Enums;

namespace MicroMarket.Services.Catalog.Models
{
    public class OutboxOperation
    {
        public Guid Id { get; init; }
        public Guid CorrelationId { get; set; } = Guid.Empty;
        public Guid AggregationId { get; init; }
        public OutboxState State { get; set; }
        public OutboxOperationType OperationType { get; set; }
        public DateTime CreateAt { get; init; } = DateTime.UtcNow;
        public DateTime ProcessedAt { get; init; }

    }
}
