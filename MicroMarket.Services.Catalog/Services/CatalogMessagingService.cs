using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Catalog.Services
{
    public class CatalogMessagingService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RpcServer<AddItemToBasket, ItemInformationResponse> _rpcServer;

        public CatalogMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _rpcServer = new RpcServer<AddItemToBasket, ItemInformationResponse>(
                messageBusService.CreateModel(),
                "catalog.item-add.rpc",
                HandleBasketItemAdd
            );
        }

        private SharedCore.RabbitMqRpc.Result<ItemInformationResponse> HandleBasketItemAdd(AddItemToBasket addItemToBasket)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                var product = dbContext.Products
                    .AsNoTracking()
                    .SingleOrDefault(p => p.Id == addItemToBasket.ItemProductId);
                if (product is null)
                    return SharedCore.RabbitMqRpc.Result<ItemInformationResponse>.Failure("Product doesn't exist");
                var response = new ItemInformationResponse()
                {
                    ItemProductId = product.Id,
                    AvailableQuantity = product.StockQuantity,
                    ItemProductName = product.Name,
                    ItemProductPrice = product.Price,
                    IsActive = product.IsActive,
                    IsDeleted = product.IsDeleted
                };
                return SharedCore.RabbitMqRpc.Result<ItemInformationResponse>.Success(response);
            }
        }
    }
}
