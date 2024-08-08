using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using RabbitMQ.Client;

namespace MicroMarket.Services.Ordering.Services
{
    public class OrderingMessagingService
    {
        private readonly IModel _model;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RpcServer<CreateOrder, CreatedOrderResponse> _orderCreatingRpcServer;

        public OrderingMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService) 
        {
            _model = messageBusService.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;

            _orderCreatingRpcServer = new RpcServer<CreateOrder, CreatedOrderResponse>(
                _model,
                "ordering.create-order.rpc",
                CreateOrderHandler
            );
                
        }

        private Result<CreatedOrderResponse> CreateOrderHandler(CreateOrder createOrder) 
        {
            throw new NotImplementedException();
        }
    }
}
