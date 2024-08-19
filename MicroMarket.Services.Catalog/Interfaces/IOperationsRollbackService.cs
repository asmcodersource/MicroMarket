namespace MicroMarket.Services.Catalog.Interfaces
{
    public interface IOperationsRollbackService
    {
        public Task ItemsClaimRollback(Guid correlationId);
    }
}
