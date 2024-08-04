using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace MicroMarket.Services.SharedCore.MessageBus.Services
{
    public class MessageBugService : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;

        public MessageBugService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory {
                HostName = "micromarket.services.rabbitmq"
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
