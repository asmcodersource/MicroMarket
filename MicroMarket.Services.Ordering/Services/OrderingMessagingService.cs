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
        private readonly RpcServer<CreateDraftOrder, CreatedDraftOrderResponse> _orderCreatingRpcServer;

        public OrderingMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService) 
        {
            _model = messageBusService.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;

            _orderCreatingRpcServer = new RpcServer<CreateDraftOrder, CreatedDraftOrderResponse>(
                _model,
                "ordering.create-draft-order.rpc",
                CreateDraftOrderHandler
            );
                
        }

        private Result<CreatedDraftOrderResponse> CreateDraftOrderHandler(CreateDraftOrder createOrder) 
        {
            return Result<CreatedDraftOrderResponse>.Failure("CreateDraftOrderHandler not implemented");
        }
    }
}
