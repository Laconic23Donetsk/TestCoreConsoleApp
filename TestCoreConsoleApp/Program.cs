using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = {"localhost:6379"},
                AbortOnConnectFail = false //to continue retrying
            });

        static async Task Main(string[] args)
        {
            var db = redis.GetDatabase();
            var pong = await db.PingAsync();
            Console.WriteLine(pong);
        }
    }
}
