using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMarket.Services.SharedCore.MessageBus.Services
{
    public interface IMessageBusService
    {
        public IModel CreateModel();
    }
}
