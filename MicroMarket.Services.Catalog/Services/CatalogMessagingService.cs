using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.Catalog.Enums;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MicroMarket.Services.Catalog.Interfaces;

namespace MicroMarket.Services.Catalog.Services
{
    public class CatalogMessagingService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RpcServer<AddItemToBasket, ItemInformationResponse> _bascketItemAddRpcServer;
        private readonly RpcServer<ClaimOrderItems, ClaimedItemsResponse> _itemsClaimRpcServer;
        private readonly EventingBasicConsumer _returnItemsConsumer;
        private readonly EventingBasicConsumer _rollbackOperationsConsumer;
        public IModel Model { get; init; }

        public CatalogMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService)
        {
            Model = messageBusService.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;
            _bascketItemAddRpcServer = new RpcServer<AddItemToBasket, ItemInformationResponse>(
                Model,
                "catalog.item-add.rpc",
                HandleBasketItemAdd
            );
            _itemsClaimRpcServer = new RpcServer<ClaimOrderItems, ClaimedItemsResponse>(
                Model,
                "catalog.items-claim.rpc",
                HandleClaimItems
            );
            _returnItemsConsumer = new EventingBasicConsumer(Model);
            _rollbackOperationsConsumer = new EventingBasicConsumer(Model);
            _returnItemsConsumer.Received += HandleItemReturn;
            _rollbackOperationsConsumer.Received += HandleOperationRollback;
            Model.ExchangeDeclare("catalog.messages.exchange", ExchangeType.Direct, true, false, null);
            Model.QueueDeclare("catalog.return-items.queue", true, false, false, null);
            Model.QueueDeclare("catalog.cancel-operation.queue", true, false, false, null);
            Model.QueueBind("catalog.return-items.queue", "catalog.messages.exchange", "return-items", null);
            Model.QueueBind("catalog.cancel-operation.queue", "catalog.messages.exchange", "cancel-operation", null);
            Model.BasicConsume("catalog.cancel-operation.queue", false, _rollbackOperationsConsumer);
            Model.BasicConsume("catalog.return-items.queue", false, _returnItemsConsumer);
        }


        private SharedCore.RabbitMqRpc.Result<ClaimedItemsResponse> HandleClaimItems(ClaimOrderItems claimOrderItems)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var productsService = scope.ServiceProvider.GetRequiredService<IProductsService>();
                var result = productsService.ClaimItems(claimOrderItems).Result;
                var convertedResult = Result<ClaimedItemsResponse>.ConvertToResult(result);
                return convertedResult;
            }
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
                        throw new InvalidOperationException("Product doesn't exist");
                    var response = new ItemInformationResponse()
                    {
                        ItemProductId = product.Id,
                        AvailableQuantity = product.StockQuantity,
                        ItemProductName = product.Name,
                        ItemProductPrice = product.Price,
                        IsActive = product.IsActive,
                        IsDeleted = product.IsDeleted
                    };
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return SharedCore.RabbitMqRpc.Result<ItemInformationResponse>.Success(response);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return SharedCore.RabbitMqRpc.Result<ItemInformationResponse>.Failure(ex.Message);
                }
            }
        }

        private void HandleItemReturn(object? model, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var json = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
            var returnItems = JsonSerializer.Deserialize<ReturnItems>(json);
            if (returnItems is null)
                return;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var productsService = scope.ServiceProvider.GetRequiredService<ProductsService>();
                productsService.ReturnItems(returnItems).Wait();
            }
            Model.BasicAck(basicDeliverEventArgs.DeliveryTag, true);
        }

        private void HandleOperationRollback(object? model, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var rollbackOperation = JsonSerializer.Deserialize<RollbackOperation>(Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray()));
            if( rollbackOperation is null) 
                return;
            switch( rollbackOperation.OperationType)
            {
                case SharedCore.MessageBus.MessageContracts.Enums.OperationType.ItemsClaimRollback:
                    
                    break;
                default:
                    return;
            }
            Model.BasicAck(basicDeliverEventArgs.DeliveryTag, true);
        }
    }
}
