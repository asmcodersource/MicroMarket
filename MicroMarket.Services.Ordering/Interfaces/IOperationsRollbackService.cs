namespace MicroMarket.Services.Ordering.Interfaces
{
    public interface IOperationsRollbackService
    {
        public Task DraftOrderCreateRollback(Guid correlationId);
    }
}
