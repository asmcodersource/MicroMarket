using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MicroMarket.Services.Catalog.Services
{
    public class CatalogMessagingService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RpcServer<AddItemToBasket, ItemInformationResponse> _bascketItemAddRpcServer;
        private readonly RpcServer<ClaimOrderItems, ClaimedItemsResponse> _itemsClaimRpcServer;
        private readonly EventingBasicConsumer _consumer;
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
            _consumer = new EventingBasicConsumer(Model);
            _consumer.Received += HandleItemReturn;
            Model.ExchangeDeclare("catalog.messages.exchange", ExchangeType.Direct, true, false, null);
            Model.QueueDeclare("catalog.return-items.queue", true, false, false, null);
            Model.QueueBind("catalog.return-items.queue", "catalog.messages.exchange", "return-items", null);
            Model.BasicConsume("catalog.return-items.queue", false, _consumer);
        }

        private SharedCore.RabbitMqRpc.Result<ClaimedItemsResponse> HandleClaimItems(ClaimOrderItems claimOrderItems)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                var transaction = dbContext.Database.BeginTransaction();
                try
                {
                    var response = new ClaimedItemsResponse();
                    foreach (var item in claimOrderItems.ItemsToClaims)
                    {
                        var product = dbContext.Products
                            .Where(p => !p.IsDeleted && p.IsActive)
                            .SingleOrDefault(p => p.Id == item.ProductId);
                        if (product is null)
                            throw new InvalidOperationException($"Product {item.ProductId} is dont exist or not active");
                        if ((product.StockQuantity - item.ProductQuantity) < 0)
                            throw new InvalidOperationException($"Not enought stock quantity of product {item.ProductId}");
                        product.StockQuantity = product.StockQuantity - item.ProductQuantity;
                        dbContext.Update(product);
                        response.ClaimedItems.Add(
                            new ClaimedItemsResponse.ClaimedItem()
                            {
                                ProductId = item.ProductId,
                                ProductName = product.Name,
                                ProductDescription = product.Description,
                                ProductPrice = product.Price,
                                ProductQuantity = item.ProductQuantity,
                            }
                        );
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return SharedCore.RabbitMqRpc.Result<ClaimedItemsResponse>.Success(response);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return SharedCore.RabbitMqRpc.Result<ClaimedItemsResponse>.Failure(ex.Message);
                }
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
                var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                var transaction = dbContext.Database.BeginTransaction();
                try
                {
                    foreach (var itemToReturn in returnItems.ItemsToReturn)
                    {
                        var product = dbContext.Products.SingleOrDefault(p => p.Id == itemToReturn.ProductId);
                        if (product is null)
                            continue;
                        product.StockQuantity += itemToReturn.ProductQuantity;
                    }
                    dbContext.SaveChanges();
                    transaction.Commit();
                    Model.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
