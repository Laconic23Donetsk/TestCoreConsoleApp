using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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

            var section = config.GetSection("RestServiceSettings");
            var settings = section.Get<Dictionary<string, RestServiceSettings>>();

            Console.WriteLine(settings["Pace Structure Service"]); 
        }
    }


    public record RestServiceSettings
    {
        public RestServiceSettings()
        {
        }

        /// <summary>
        /// Full Url to REST service endpoint
        /// </summary>
        public string? Url { get; init; }

        /// <summary>
        /// Full qualified interface name of REST service
        /// </summary>
        public string? ServiceContract { get; init; }

        public HealthCheckSettings HealthCheckSettings { get; init; }

    }

    /// <summary>
    /// Setting for health check configuration
    /// </summary>
    public record HealthCheckSettings
    {
        /// <summary>
        /// Default URI for health check endpoint
        /// </summary>
        public const string DefaultHealthCheckUri = "/health";

        /// <summary>
        /// To check health of REST service
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Relative path to health check endpoint
        /// </summary>
        public string RelativePath { get; set; } = DefaultHealthCheckUri;
    }
}
