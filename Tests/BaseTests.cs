using Microsoft.Playwright;
using PlaywrightAutomation.Helpers;

namespace PlaywrightAutomation.Tests
{
    public class BaseTest : PageTest
    {
        protected AppSettings Settings { get; private set; } = null!;
        protected new string BrowserName { get; private set; } = "chromium";
        protected string? DeviceName { get; private set; }
        protected bool IsMobileTest => !string.IsNullOrEmpty(DeviceName);
        
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _customPage;

        public BaseTest(string browserName = "chromium", string? deviceName = null)
        {
            BrowserName = browserName;
            DeviceName = deviceName;
        }

        public override BrowserNewContextOptions ContextOptions()
        {
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
            
            if (IsMobileTest)
            {
                await SetupMobileBrowser();
            }
            else
            {
                await SetupDesktopBrowser();
            }
            
            _customPage = await _context!.NewPageAsync();
            _customPage.SetDefaultTimeout(Settings.DefaultTimeout);
        }

        /// <summary>
        /// Setup browser for Mobile tests with device emulation
        /// </summary>
        private async Task SetupMobileBrowser()
        {
            if (string.IsNullOrEmpty(DeviceName))
            {
                throw new InvalidOperationException("DeviceName is required for mobile tests");
            }

            if (!_playwright!.Devices.ContainsKey(DeviceName))
            {
                throw new ArgumentException($"Device '{DeviceName}' not found in Playwright devices. Available devices: {string.Join(", ", _playwright.Devices.Keys.Take(10))}");
            }

            var device = _playwright.Devices[DeviceName];

            var requestedBrowser = NormalizeBrowserName(BrowserName);

            var deviceBrowserType = InferBrowserTypeFromDevice(DeviceName);

            ValidateBrowserForDevice(DeviceName, requestedBrowser, deviceBrowserType);

            _browser = await LaunchBrowserAsync(requestedBrowser, deviceBrowserType);

            _context = await _browser.NewContextAsync(device);
            
            Log($"Mobile Test - Browser: {requestedBrowser}, Device: {DeviceName}, Device Browser Type: {deviceBrowserType}");
        }

        /// <summary>
        /// Setup browser for Desktop tests
        /// </summary>
        private async Task SetupDesktopBrowser()
        {
            var browserName = NormalizeBrowserName(BrowserName);
            _browser = await LaunchBrowserAsync(browserName, browserName);
            
            var contextOptions = ContextOptions();
            _context = await _browser.NewContextAsync(contextOptions);
            
            Log($"Desktop Test - Browser: {browserName}");
        }

        /// <summary>
        /// Launch browser based on browser name
        /// </summary>
        private async Task<IBrowser> LaunchBrowserAsync(string browserName, string? preferredBrowserType = null)
        {
            BrowserTypeLaunchOptions launchOptions;
            IBrowser browser;

            switch (browserName.ToLower())
            {
                case "firefox":
                    launchOptions = new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.Headless,
                        Timeout = 60000
                    };
                    browser = await _playwright!.Firefox.LaunchAsync(launchOptions);
                    break;

                case "webkit":
                    launchOptions = new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.Headless,
                        Timeout = 60000
                    };
                    browser = await _playwright!.Webkit.LaunchAsync(launchOptions);
                    break;

                case "chrome":
                case "msedge":
                case "edge":
                case "chromium":
                default:
                    launchOptions = new BrowserTypeLaunchOptions
                    {
                        Headless = Settings.Headless,
                        Args = Settings.Headless ? null : new[] { "--disable-blink-features=AutomationControlled" }
                    };

                    if (browserName.ToLower() == "chrome")
                    {
                        launchOptions.Channel = "chrome";
                    }
                    else if (browserName.ToLower() == "msedge" || browserName.ToLower() == "edge")
                    {
                        launchOptions.Channel = "msedge";
                    }

                    browser = await _playwright!.Chromium.LaunchAsync(launchOptions);
                    break;
            }

            return browser;
        }

        /// <summary>
        /// Normalize browser name to handle different names
        /// </summary>
        private string NormalizeBrowserName(string browserName)
        {
            return browserName.ToLower() switch
            {
                "chrome" or "chromium" => "chromium",
                "msedge" or "edge" => "msedge",
                "firefox" => "firefox",
                "webkit" or "safari" => "webkit",
                _ => browserName.ToLower()
            };
        }

        /// <summary>
        /// Infer browser type from device name
        /// </summary>
        private string InferBrowserTypeFromDevice(string deviceName)
        {
            // iOS devices usually use webkit
            if (deviceName.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || 
                deviceName.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                return "webkit";
            }
            // Android devices usually use chromium
            return "chromium";
        }

        /// <summary>
        /// Validate browser type is compatible with device
        /// </summary>
        private void ValidateBrowserForDevice(string deviceName, string requestedBrowser, string deviceDefaultBrowser)
        {
            // iOS devices (iPhone, iPad) usually use webkit
            if (deviceName.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || 
                deviceName.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                if (requestedBrowser != "webkit" && requestedBrowser != "chromium")
                {
                    Log($"Warning: iOS device '{deviceName}' typically uses 'webkit' or 'chromium', but '{requestedBrowser}' was requested. Device default: {deviceDefaultBrowser}");
                }
            }
            // Android devices usually use chromium
            else if (deviceName.Contains("Pixel", StringComparison.OrdinalIgnoreCase) || 
                     deviceName.Contains("Galaxy", StringComparison.OrdinalIgnoreCase) ||
                     deviceName.Contains("Android", StringComparison.OrdinalIgnoreCase))
            {
                if (requestedBrowser != "chromium" && requestedBrowser != "chrome")
                {
                    Log($"Warning: Android device '{deviceName}' typically uses 'chromium' or 'chrome', but '{requestedBrowser}' was requested. Device default: {deviceDefaultBrowser}");
                }
            }

            // Log if browser requested is different from device default
            if (requestedBrowser != deviceDefaultBrowser)
            {
                Log($"Info: Using '{requestedBrowser}' instead of device default '{deviceDefaultBrowser}' for device '{deviceName}'");
            }
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
