using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MicroMarket.Services.Catalog.Services
{
    public class OperationsRollbackService : IOperationsRollbackService
    {
        private readonly CatalogDbContext _catalogDbContext;

        public OperationsRollbackService(CatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }

        public async Task ItemsClaimRollback(Guid correlationId)
        {
            var transaction = await _catalogDbContext.Database.BeginTransactionAsync();
            try
            {
                var operation =  await _catalogDbContext.OutboxOperations
                    .Where(o => o.CorrelationId == correlationId && o.State != Enums.OutboxState.RolledBack)
                    .SingleOrDefaultAsync();
                if (operation is null)
                    return;
                var claimedItemsOperation = await _catalogDbContext.ItemsClaimOperations
                    .Where(o => o.Id == operation.AggregationId)
                    .SingleOrDefaultAsync();
                if (claimedItemsOperation is null)
                    throw new ApplicationException();
                var productsService = new ProductsService(_catalogDbContext, null);
                var itemsToReturn = claimedItemsOperation.ItemsToClaim
                    .Select(i => new ReturnItems.ItemToReturn()
                    {
                        ProductId = i.ProductId,
                        ProductQuantity = i.ProductQuantity
                    }
                ).ToList();

                await productsService.ReturnItems(new ReturnItems()
                {
                    ItemsToReturn = itemsToReturn,
                });

                operation.State = Enums.OutboxState.RolledBack;
                await _catalogDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            } catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
