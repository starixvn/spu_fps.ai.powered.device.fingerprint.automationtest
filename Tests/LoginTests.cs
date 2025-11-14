using PlaywrightAutomation.Helpers;
using PlaywrightAutomation.Pages;

namespace PlaywrightAutomation.Tests
{
    [TestFixture("chrome")]
    [TestFixture("msedge")]
    //[TestFixture("brave")]
    //[TestFixture("firefox")]
    [TestFixture("webkit")]
    public class LoginTests : BaseTest
    {
        private LoginPage _loginPage = null!;

        public LoginTests(string browserName) : base(browserName)
        {
        }

        [SetUp]
        public async Task Setup()
        {
            await BaseSetup();
            _loginPage = new LoginPage(Page);
            Log($"Running test on browser: {BrowserName}");
        }

        [Test]
        [Category("Login")]
        [Category("Smoke")]
        public async Task Login_WithValidCredentials_ShouldSucceed()
        {
            await _loginPage.NavigateToLoginPage(Settings.BaseUrl);
            await _loginPage.Login(Settings.Credentials.Username, Settings.Credentials.Password);
            await _loginPage.WaitForPageLoad();

            var isLoginSuccessful = await _loginPage.IsLoginSuccessful();
            Assert.That(isLoginSuccessful, Is.True, $"Login should succeed on {BrowserName}");
        }
    }
}

