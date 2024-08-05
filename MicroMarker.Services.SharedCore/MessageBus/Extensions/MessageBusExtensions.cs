using MicroMarket.Services.SharedCore.MessageBus.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMarket.Services.SharedCore.MessageBus.Extensions
{
    public static class MessageBusExtensions
    {
        public static void AddMessageBusService(this IServiceCollection services)
        {
            services.AddSingleton<MessageBusService>();
        }
    }
}
