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
        private readonly CatalogMessagingService _catalogMessagingService;

        public OperationsRollbackService(CatalogDbContext catalogDbContext, CatalogMessagingService catalogMessagingService)
        {
            _catalogDbContext = catalogDbContext;
            _catalogMessagingService = catalogMessagingService;
        }

        public async Task ItemsClaimRollback(Guid correlationId)
        {
            var operation =  await _catalogDbContext.OutboxOperations
                .Where(o => o.CorrelationId == correlationId && o.State != Enums.OutboxState.RolledBack)
                .SingleOrDefaultAsync();
            if (operation is null)
                return;
            if (operation.OperationType != Enums.OutboxOperationType.ItemsClaim)
                throw new InvalidOperationException();
            var claimedItemsOperation = await _catalogDbContext.ItemsClaimOperations
                .Where(o => o.Id == operation.AggregationId)
                .Include(o => o.ItemsToClaim)
                .SingleOrDefaultAsync();
            if (claimedItemsOperation is null)
                throw new ApplicationException();
            var productsService = new ProductsService(_catalogDbContext, _catalogMessagingService);
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
        }
    }
}
