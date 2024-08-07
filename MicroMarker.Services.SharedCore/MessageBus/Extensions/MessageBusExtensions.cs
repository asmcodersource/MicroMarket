using MicroMarket.Services.SharedCore.MessageBus.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MicroMarket.Services.SharedCore.MessageBus.Extensions
{
    public static class MessageBusExtensions
    {
        public static void AddMessageBusService(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBusService, MessageBusService>();
        }
    }
}
