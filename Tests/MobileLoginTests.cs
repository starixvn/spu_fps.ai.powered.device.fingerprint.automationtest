using PlaywrightAutomation.Pages;

namespace PlaywrightAutomation.Tests
{
    [TestFixture("chrome", "iPhone 13")]
    [TestFixture("chrome", "iPhone 13 Pro")]
    [TestFixture("chrome", "iPhone 12")]
    [TestFixture("chrome", "iPhone SE")]
    [TestFixture("webkit", "iPhone 13")]
    [TestFixture("webkit", "iPhone 13 Pro")]
    
    [TestFixture("chrome", "iPad Pro")]
    [TestFixture("webkit", "iPad Pro")]
    [TestFixture("webkit", "iPad Pro 11")]
    
    [TestFixture("chrome", "Pixel 5")]
    [TestFixture("chrome", "Pixel 4")]
    [TestFixture("chrome", "Galaxy S9+")]
    [TestFixture("chrome", "Galaxy Tab S4")]
    
    public class MobileLoginTests : BaseTest
    {
        private LoginPage _loginPage = null!;

        public MobileLoginTests(string browserName, string deviceName) : base(browserName, deviceName)
        {
        }

        [SetUp]
        public async Task Setup()
        {
            await BaseSetup();
            _loginPage = new LoginPage(Page);
        }

        [Test]
        [Category("Login")]
        [Category("Mobile")]
        [Category("Smoke")]
        public async Task Login_WithValidCredentials_ShouldSucceed()
        {
            await _loginPage.NavigateToLoginPage(Settings.BaseUrl);
            await _loginPage.Login(Settings.Credentials.Username, Settings.Credentials.Password);
            await _loginPage.WaitForPageLoad();

            var isLoginSuccessful = await _loginPage.IsLoginSuccessful();
            Assert.That(isLoginSuccessful, Is.True, $"Login should succeed on {BrowserName} on {DeviceName}");
        }
    }
}

