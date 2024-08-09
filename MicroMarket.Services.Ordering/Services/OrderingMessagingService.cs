using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.Ordering.DbContexts;
using RabbitMQ.Client;
using MicroMarket.Services.Ordering.Models;
using MicroMarket.Services.Ordering.Interfaces;
using CSharpFunctionalExtensions;

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

        private SharedCore.RabbitMqRpc.Result<CreatedDraftOrderResponse> CreateDraftOrderHandler(CreateDraftOrder createOrder) 
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    IDraftOrdersService draftOrdersService = scope.ServiceProvider.GetRequiredService<IDraftOrdersService>();
                    var draftOrderCreateResult = draftOrdersService.CreateDraftOrder(createOrder).Result;
                    if( draftOrderCreateResult.IsFailure ) 
                        return SharedCore.RabbitMqRpc.Result<CreatedDraftOrderResponse>.Failure(draftOrderCreateResult.Error);
                    var response = new CreatedDraftOrderResponse(draftOrderCreateResult.Value.Id);
                    return SharedCore.RabbitMqRpc.Result<CreatedDraftOrderResponse>.Success(response);
                }
            }
            catch (Exception ex)
            {
                return SharedCore.RabbitMqRpc.Result<CreatedDraftOrderResponse>.Failure(ex.Message);
            }
        }
    }
}
