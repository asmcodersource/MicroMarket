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
        private readonly RabbitMQ.Client.IModel _model;
        private readonly EventingBasicConsumer _consumer;
        public RpcClient<AddItemToBasket, ItemInformationResponse> ItemAddRpcClient { get; init; }

        public BasketMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _model = messageBusService.CreateModel();
            _consumer = new EventingBasicConsumer(_model);
            _consumer.Received += ProductUpdateEventHandler;

            ItemAddRpcClient = new RpcClient<AddItemToBasket, ItemInformationResponse>(
                _model,
                "catalog.item-add.rpc",
                "basket.item-add-rpc"
            );

            _model.ExchangeDeclare("catalog.messages.exchange", ExchangeType.Direct, true, false, null);
            _model.QueueDeclare("basket.product-update.queue", true, false, false, null);
            _model.QueueBind("basket.product-update.queue", "catalog.messages.exchange", "product-update", null);
            _model.BasicConsume("basket.product-update.queue", true, _consumer);
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
    }
}
