using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MicroMarket.Services.Basket.Services
{
    public class BasketMessagingService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventingBasicConsumer _consumer;

        public RabbitMQ.Client.IModel Model { get; init; }
        public RpcClient<AddItemToBasket, ItemInformationResponse> ItemAddRpcClient { get; init; }
        public RpcClient<ClaimOrderItems, ClaimedItemsResponse> ClaimItemsRpcClient { get; init; }
        public RpcClient<CreateDraftOrder, CreatedDraftOrderResponse> CreateDraftOrderRpcClient { get; init; }


        public BasketMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            Model = messageBusService.CreateModel();
            _consumer = new EventingBasicConsumer(Model);
            _consumer.Received += ProductUpdateEventHandler;

            ItemAddRpcClient = new RpcClient<AddItemToBasket, ItemInformationResponse>(
                Model,
                "catalog.item-add.rpc",
                "basket.item-add.rpc"
            );

            ClaimItemsRpcClient = new RpcClient<ClaimOrderItems, ClaimedItemsResponse>(
                Model,
                "catalog.items-claim.rpc",
                "basket.items-claim.rpc"
            );

            CreateDraftOrderRpcClient = new RpcClient<CreateDraftOrder, CreatedDraftOrderResponse>(
                Model,
                "ordering.create-draft-order.rpc",
                "basket.create-draft-order.rpc"
            );

            Model.ExchangeDeclare("catalog.messages.exchange", ExchangeType.Direct, true, false, null);
            Model.QueueDeclare("basket.product-update.queue", true, false, false, null);
            Model.QueueBind("basket.product-update.queue", "catalog.messages.exchange", "product-update", null);
            Model.BasicConsume("basket.product-update.queue", true, _consumer);
        }

        private void ProductUpdateEventHandler(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var json = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
            var updateEvent = JsonSerializer.Deserialize<ItemInformationResponse>(json);
            if (updateEvent is null)
                return;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
                var product = dbContext.Products
                    .Where(p => p.CatalogProductId == updateEvent.ItemProductId)
                    .SingleOrDefault();
                if (product is null)
                    return;
                product.Price = updateEvent.ItemProductPrice;
                product.Name = updateEvent.ItemProductName;
                product.Quantity = updateEvent.AvailableQuantity;
                product.IsActive = updateEvent.IsActive;
                product.IsDeleted = updateEvent.IsDeleted;
                dbContext.SaveChanges();
            }
        }

        public void ReturnItemsToCatalog(ReturnItems returnItems)
        {
            var json = JsonSerializer.Serialize(returnItems);
            Model.BasicPublish("catalog.messages.exchange", "return-items", null, Encoding.UTF8.GetBytes(json));
        }
    }
}
