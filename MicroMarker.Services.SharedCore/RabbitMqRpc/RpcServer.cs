using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using System.Threading.Channels;

namespace MicroMarket.Services.SharedCore.RabbitMqRpc
{
    public class RpcServer<RequestType, ResponseType>
    {
        private readonly IModel _channel;
        public event Func<RequestType, Result<ResponseType>> OnRequest;

        public RpcServer(IModel channel, string rpcQueueName, Func<RequestType, Result<ResponseType>> onRequest)
        {
            OnRequest += onRequest;
            _channel = channel;
            _channel.QueueDeclare(queue: rpcQueueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 100, global: false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += MessageHandler;

            _channel.BasicConsume(queue: rpcQueueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        public void SendResponse(Result<ResponseType> response, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var json = JsonSerializer.Serialize(response);
            var responseBytes = Encoding.UTF8.GetBytes(json);
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = basicDeliverEventArgs.BasicProperties.CorrelationId;
            _channel.BasicPublish(exchange: string.Empty,
                                 routingKey: basicDeliverEventArgs.BasicProperties.ReplyTo,
                                 basicProperties: replyProps,
                                 body: responseBytes);
            _channel.BasicAck(deliveryTag: basicDeliverEventArgs.DeliveryTag, multiple: false);
        }

        private void MessageHandler(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var body = basicDeliverEventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            RequestType? request = JsonSerializer.Deserialize<RequestType>(json);
            if (request is not null)
            {
                var result = OnRequest.Invoke(request);
                SendResponse(result, basicDeliverEventArgs);
            }
            else
            {
                var result = Result<ResponseType>.Failure("Bad request failure");
                SendResponse(result, basicDeliverEventArgs);
            }
        }
    }
}
