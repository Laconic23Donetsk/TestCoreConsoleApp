using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();

            var section = config.GetSection("InfoLogExceptionFilter");
            var infoLogExceptionFilterSettings = section.Get<InfoLogExceptionFilterSettings[]>();
            //var settings = JsonSerializer.Deserialize<InfoLogExceptionFilterSettings>(json);

            var jsonStr = JsonSerializer.Serialize(infoLogExceptionFilterSettings);
            Console.WriteLine(jsonStr);
        }
    }
        
    public class InfoLogExceptionFilterSettings
    {
        public string? Path { get; set; }
        public string? Type { get; set; }
        public string? Code { get; set; }
    }
}
