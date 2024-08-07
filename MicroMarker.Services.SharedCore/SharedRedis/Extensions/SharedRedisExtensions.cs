using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MicroMarket.Services.SharedCore.SharedRedis.Extensions
{
    public static class SharedRedisExtensions
    {
        public static void AddSharedRedisDistributedCache(this IServiceCollection services, IConfiguration configuration, string instance = "SharedInstance")
        {
            var redisConfiguration = configuration["RedisConnectionString"];
            if (redisConfiguration is null)
                throw new InvalidOperationException("Redis configuration string must be defined in .env file");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration;
                options.InstanceName = instance;
            });
        }
    }
}
