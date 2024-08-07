using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace MicroMarket.Services.Catalog.Services
{
    public class CatalogMessagingService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RpcServer<AddItemToBasket, ItemInformationResponse> _rpcServer;
        public IModel Model { get; init; }

        public CatalogMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService)
        {
            Model = messageBusService.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;
            _rpcServer = new RpcServer<AddItemToBasket, ItemInformationResponse>(
                Model,
                "catalog.item-add.rpc",
                HandleBasketItemAdd
            );
            Model.ExchangeDeclare("catalog.messages.exchange", ExchangeType.Direct, true, false, null);
        }

        private SharedCore.RabbitMqRpc.Result<ItemInformationResponse> HandleBasketItemAdd(AddItemToBasket addItemToBasket)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                var transaction = dbContext.Database.BeginTransaction();
                try
                {
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
                    transaction.Commit();
                    return SharedCore.RabbitMqRpc.Result<ItemInformationResponse>.Success(response);
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    return SharedCore.RabbitMqRpc.Result<ItemInformationResponse>.Failure(ex.Message);
                }
            }
        }
    }
}
