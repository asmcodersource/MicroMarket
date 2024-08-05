using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;

namespace MicroMarket.Services.SharedCore.RabbitMqRpc
{
    public class RpcClient<RequestType, ResponseType> : IDisposable
    {
        private readonly string _rpcQueueName;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<CSharpFunctionalExtensions.Result<ResponseType>>> _callbackMapper = new();

        public RpcClient(IModel channel, string rpcQueueName)
        {
            _rpcQueueName = rpcQueueName;
            _channel = channel;
            _replyQueueName = _channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                    return;
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                Result<ResponseType>? response = JsonSerializer.Deserialize<Result<ResponseType>>(json);
                if (response is null)
                    throw new InvalidDataException();
                tcs.TrySetResult(Result<ResponseType>.ConvertToCSharpFunctionalExtensionsResult(response));
            };

            _channel.BasicConsume(consumer: consumer,
                                 queue: _replyQueueName,
                                 autoAck: true);
        }

        public Task<CSharpFunctionalExtensions.Result<ResponseType>> CallAsync(RequestType request, CancellationToken cancellationToken = default)
        {
            IBasicProperties props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;
            var json = JsonSerializer.Serialize(request);
            var messageBytes = Encoding.UTF8.GetBytes(json);
            var tcs = new TaskCompletionSource<CSharpFunctionalExtensions.Result<ResponseType>>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _channel.BasicPublish(exchange: string.Empty,
                                 routingKey: _rpcQueueName,
                                 basicProperties: props,
                                 body: messageBytes);

            cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out _));
            return tcs.Task;
        }

        public void Dispose()
        {
            _channel.Close();
            GC.SuppressFinalize(true);
        }
    }
}
