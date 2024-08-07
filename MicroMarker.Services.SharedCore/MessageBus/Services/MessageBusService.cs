using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace MicroMarket.Services.SharedCore.MessageBus.Services
{
    public class MessageBusService : IDisposable, IMessageBusService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;

        public MessageBusService(IConfiguration configuration)
        {
            _configuration = configuration;
            var rabbitHost = _configuration["RabbitHost"];
            if (rabbitHost is null)
                throw new InvalidOperationException();
            _connectionFactory = new ConnectionFactory
            {
                HostName = rabbitHost
            };
            _connection = _connectionFactory.CreateConnection();
        }

        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }

        void IDisposable.Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
