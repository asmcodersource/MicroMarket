using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;
using MicroMarket.Services.Ordering.DbContexts;

namespace MicroMarket.Services.Ordering.Services
{
    public class OrderingMessagingService
    {
        public readonly IModel Model;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RpcServer<CreateDraftOrder, CreatedDraftOrderResponse> _orderCreatingRpcServer;
        private readonly EventingBasicConsumer _rollbackOperationsConsumer;

        public OrderingMessagingService(IServiceScopeFactory serviceScopeFactory, IMessageBusService messageBusService)
        {
            Model = messageBusService.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;

            _orderCreatingRpcServer = new RpcServer<CreateDraftOrder, CreatedDraftOrderResponse>(
                Model,
                "ordering.create-draft-order.rpc",
                CreateDraftOrderHandler
            );

            _rollbackOperationsConsumer = new EventingBasicConsumer(Model);
            _rollbackOperationsConsumer.Received += HandleOperationCancel;
            Model.ExchangeDeclare("ordering.messages.exchange", ExchangeType.Direct, true, false, null);
            Model.QueueDeclare("ordering.rollback-operation.queue", true, false, false, null);
            Model.QueueBind("ordering.rollback-operation.queue", "ordering.messages.exchange", "rollback-operation", null);
            Model.BasicConsume("ordering.rollback-operation.queue", false, _rollbackOperationsConsumer);
        }

        private SharedCore.RabbitMqRpc.Result<CreatedDraftOrderResponse> CreateDraftOrderHandler(CreateDraftOrder createOrder)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    IDraftOrdersService draftOrdersService = scope.ServiceProvider.GetRequiredService<IDraftOrdersService>();
                    var draftOrderCreateResult = draftOrdersService.CreateDraftOrder(createOrder).Result;
                    if (draftOrderCreateResult.IsFailure)
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

        public void ReturnItemsToCatalog(ReturnItems returnItems)
        {
            var json = JsonSerializer.Serialize(returnItems);
            Model.BasicPublish("catalog.messages.exchange", "return-items", null, Encoding.UTF8.GetBytes(json));
        }

        private void HandleOperationCancel(object? model, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var rollbackOperation = JsonSerializer.Deserialize<RollbackOperation>(Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray()));
            if (rollbackOperation is null)
                return;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
                var rollbackService = new OperationsRollbackService(dbContext);
                switch (rollbackOperation.OperationType)
                {
                    case SharedCore.MessageBus.MessageContracts.Enums.OperationType.DraftOrderRollback:
                        rollbackService.DraftOrderCreateRollback(rollbackOperation.CorrelationId).Wait();
                        break;
                }
                Model.BasicAck(basicDeliverEventArgs.DeliveryTag, true);
            }
        }
    }
}
