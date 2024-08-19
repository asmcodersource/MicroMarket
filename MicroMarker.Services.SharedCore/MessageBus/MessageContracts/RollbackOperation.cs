using MicroMarket.Services.SharedCore.MessageBus.MessageContracts.Enums;

namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class RollbackOperation
    {
        public OperationType OperationType { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
