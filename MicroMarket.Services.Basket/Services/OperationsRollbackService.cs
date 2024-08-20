using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.Basket.Interfaces;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace MicroMarket.Services.Basket.Services
{
    public class OperationsRollbackService : IOperationsRollbackService
    {
        private readonly BasketDbContext _dbContext;
        private readonly BasketMessagingService _messagingService;

        public OperationsRollbackService(BasketDbContext basketDbContext, BasketMessagingService basketMessagingService)
        {
            _dbContext = basketDbContext;
            _messagingService = basketMessagingService;
        }

        public void MarkExecutingAsFailured()
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                _dbContext.OutboxOperations
                    .Where(o => o.State == Enums.OutboxState.Executing)
                    .ExecuteUpdate(
                    o => o.SetProperty(
                        o => o.State, 
                        o => o.State == Enums.OutboxState.Executing ? Enums.OutboxState.Failure : o.State)
                    );
                _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task OrderCreatingRollback(Guid correlationId)
        {
            var operation = await _dbContext.OutboxOperations
                .Where(o => o.CorrelationId == correlationId && o.State != Enums.OutboxState.RolledBack)
                .SingleOrDefaultAsync();
            if (operation is null)
                return;
            if (operation.OperationType != Enums.OutboxOperationType.OrderCreating)
                throw new InvalidOperationException();
            RollbackItemsClaim(correlationId);
            RollbackDraftOrderCreating(correlationId);
            operation.State = Enums.OutboxState.RolledBack;
            await _dbContext.SaveChangesAsync();
        }

        private void RollbackItemsClaim(Guid correlationId)
        {
            var rollbackRequest = new RollbackOperation()
            {
                CorrelationId = correlationId,
                OperationType = SharedCore.MessageBus.MessageContracts.Enums.OperationType.ItemsClaimRollback
            };
            _messagingService.Model.BasicPublish(
                "catalog.messages.exchange",
                "rollback-operation",
                false,
                null,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rollbackRequest))
            );
        }

        private void RollbackDraftOrderCreating(Guid correlationId)
        {
            var rollbackRequest = new RollbackOperation()
            {
                CorrelationId = correlationId,
                OperationType = SharedCore.MessageBus.MessageContracts.Enums.OperationType.DraftOrderRollback
            };
            _messagingService.Model.BasicPublish(
                "ordering.messages.exchange",
                "rollback-operation",
                false,
                null,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rollbackRequest))
            );
        }
    }
}
