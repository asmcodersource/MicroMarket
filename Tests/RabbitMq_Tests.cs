using MicroMarket.Services.SharedCore.RabbitMqRpc;
using RabbitMQ.Client;

namespace Tests
{
    public class RabbitMq_Tests
    {
        [Fact]
        public async void Rpc_Communication_Testing()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = connectionFactory.CreateConnection();
            RpcServer<string, string> rpcServer = new RpcServer<string, string>(
                connection.CreateModel(),
                "test.service.rpc.tests",
                (str) => Result<string>.Success(str)
            );
            RpcClient<string, string> rpcClient = new RpcClient<string, string>(
               connection.CreateModel(),
               "test.service.rpc.tests"
           );
            for (int i = 0; i < 10000; i++)
            {
                var uniqValue = Guid.NewGuid().ToString();
                var (response, _) = await rpcClient.CallAsync(uniqValue);
                Assert.True(response.IsSuccess);
                Assert.Equal(uniqValue, response.Value);
            }
        }
    }
}