using Microsoft.Playwright;

public class PlaywrightHelper
{
    public static async Task<string> GetRenderedHtmlAsync(string url)
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Timeout = 60000
        });

        var page = await browser.NewPageAsync();
        await page.GotoAsync(url, new PageGotoOptions { Timeout = 60000 });

        // Wait for the productList to be available
        await page.WaitForSelectorAsync("div.productList", new PageWaitForSelectorOptions
        {
            Timeout = 30000
        });

        return await page.ContentAsync();
    }
}
