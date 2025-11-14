using Microsoft.Extensions.Configuration;
using PlaywrightAutomation;

namespace PlaywrightAutomation.Helpers
{
    /// <summary>
    /// Configuration helper - Singleton pattern for loading app settings
    /// </summary>
    public class ConfigurationHelper
    {
        private static ConfigurationHelper? _instance;
        private static readonly object _lock = new object();
        
        public AppSettings Settings { get; }

        private ConfigurationHelper()
        {
            var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "LOCAL";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Settings = new AppSettings(configuration);

            Console.WriteLine($"[CONFIG] Environment: {environment}");
            Console.WriteLine($"[CONFIG] Base URL: {Settings.BaseUrl}");
        }

        public static ConfigurationHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ConfigurationHelper();
                    }
                }
                return _instance;
            }
        }
    }
}

