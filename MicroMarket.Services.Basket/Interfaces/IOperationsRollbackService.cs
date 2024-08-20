namespace MicroMarket.Services.Basket.Interfaces
{
    public interface IOperationsRollbackService
    {
        public Task OrderCreatingRollback(Guid correlationId);
        public void MarkExecutingAsFailured();
    }
}
