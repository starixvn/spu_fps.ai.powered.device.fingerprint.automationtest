using Microsoft.Extensions.Configuration;

namespace PlaywrightAutomation
{
    public interface IAppSettings
    {
        string BaseUrl { get; set; }

        string ApiUrl { get; set; }

        int DefaultTimeout { get; set; }

        bool Headless { get; set; }

        string Browser { get; set; }

        List<string> Browsers { get; set; }

        TestCredentials Credentials { get; set; }
    }

    public class AppSettings : IAppSettings
    {
       private readonly IConfiguration configuration;

       public AppSettings(IConfiguration configuration)
       {
           this.configuration = configuration;

            BaseUrl = configuration["BaseUrl"] ?? string.Empty;
            ApiUrl = configuration["ApiUrl"] ?? string.Empty;
            DefaultTimeout = int.Parse(configuration["DefaultTimeout"] ?? "30000");
            Headless = bool.Parse(configuration["Headless"] ?? "true");
            Browser = configuration["Browser"] ?? "chromium";
            Browsers = configuration.GetSection("Browsers").Get<List<string>>() ?? new List<string> { "chromium" };
            Credentials = configuration.GetSection("Credentials").Get<TestCredentials>() ?? new TestCredentials();
        }
        
        public string BaseUrl { get; set; } = string.Empty;
        
        public string ApiUrl { get; set; } = string.Empty;

        public int DefaultTimeout { get; set; } = 30000;

        public bool Headless { get; set; } = true;

        public string Browser { get; set; } = "chromium";

        public List<string> Browsers { get; set; } = new List<string> { "chromium" };

        public TestCredentials Credentials { get; set; } = new TestCredentials();
    }

    /// <summary>
    /// Test user credentials
    /// </summary>
    public class TestCredentials
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

