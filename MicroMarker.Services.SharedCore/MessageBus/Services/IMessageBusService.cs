using RabbitMQ.Client;

namespace MicroMarket.Services.SharedCore.MessageBus.Services
{
    public interface IMessageBusService
    {
        public IModel CreateModel();
    }
}
