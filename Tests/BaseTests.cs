using Microsoft.Playwright;
using PlaywrightAutomation.Helpers;

namespace PlaywrightAutomation.Tests
{
    public class BaseTest : PageTest
    {
        protected AppSettings Settings { get; private set; } = null!;
        protected new string BrowserName { get; private set; } = "chromium";
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _customPage;

        public BaseTest(string browserName = "chromium")
        {
            BrowserName = browserName;
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            var settings = ConfigurationHelper.Instance.Settings;
            return new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                HasTouch = false,
                IsMobile = false,
                Locale = "en-US",
                TimezoneId = "America/New_York"
            };
        }

        [SetUp]
        public virtual async Task BaseSetup()
        {
            Settings = ConfigurationHelper.Instance.Settings;
            
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            
            BrowserTypeLaunchOptions launchOptions;
            string browserNameLower = BrowserName.ToLower();
            
            if (browserNameLower == "firefox")
            {
                launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = Settings.Headless,
                    Timeout = 60000
                };
                _browser = await _playwright.Firefox.LaunchAsync(launchOptions);
            }
            else if (browserNameLower == "webkit")
            {
                launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = Settings.Headless,
                    Timeout = 60000
                };
                _browser = await _playwright.Webkit.LaunchAsync(launchOptions);
            }
            else
            {
                launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = Settings.Headless,
                    Args = Settings.Headless ? null : new[] { "--disable-blink-features=AutomationControlled" }
                };
                
                if (browserNameLower == "chrome")
                {
                    launchOptions.Channel = "chrome";
                }
                else if (browserNameLower == "msedge" || browserNameLower == "edge")
                {
                    launchOptions.Channel = "msedge";
                }
                
                _browser = await _playwright.Chromium.LaunchAsync(launchOptions);
            }
            
            var contextOptions = ContextOptions();
            _context = await _browser.NewContextAsync(contextOptions);
            
            _customPage = await _context.NewPageAsync();
            _customPage.SetDefaultTimeout(Settings.DefaultTimeout);
        }
        
        protected new IPage Page => _customPage ?? base.Page;

        [TearDown]
        public virtual async Task BaseTearDown()
        {
            if (_customPage != null)
            {
                await _customPage.CloseAsync();
                _customPage = null;
            }
            if (_context != null)
            {
                await _context.CloseAsync();
                _context = null;
            }
            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser = null;
            }
            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
            }
        }

        protected void Log(string message) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}
