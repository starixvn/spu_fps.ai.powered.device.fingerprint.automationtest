using Microsoft.Playwright;

namespace PlaywrightAutomation.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateToLoginPage(string baseUrl)
        {
            await _page.GotoAsync(baseUrl);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task EnterUsername(string username)
        {
            await _page.FillAsync("#txtUserName", username);
        }

        public async Task ClickVerifyButton()
        {
            await _page.ClickAsync("#btnVerify");
            await _page.WaitForSelectorAsync("#txtPassWord", new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
        }

        public async Task EnterPassword(string password)
        {
            await _page.FillAsync("#txtPassWord", password);
        }

        public async Task ClickLoginButton()
        {
            await _page.ClickAsync("#btnLogin");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task Login(string username, string password)
        {
            await EnterUsername(username);
            await ClickVerifyButton();
            await EnterPassword(password);
            await ClickLoginButton();
        }

        public async Task WaitForPageLoad()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task<bool> IsLoginSuccessful()
        {
            await Task.Delay(2000);
            
            var formExists = await _page.Locator("#formLogin").CountAsync() > 0;
            var currentUrl = _page.Url;
            
            return !formExists || !currentUrl.Contains("login", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> IsErrorMessageVisible()
        {
            var errorMsg = _page.Locator("#errmsg");
            var isVisible = await errorMsg.IsVisibleAsync();
            var text = await errorMsg.TextContentAsync();
            return isVisible && !string.IsNullOrWhiteSpace(text);
        }
    }
}

