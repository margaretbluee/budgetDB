using System.Text;
using Microsoft.Playwright;

public class SklavenitisPlaywrightHelper
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

        try
        {
            await page.WaitForSelectorAsync("section.productList", new PageWaitForSelectorOptions
            {
                Timeout = 30000
            });
        }
        catch (TimeoutException)
        {
            Console.WriteLine("⚠️ Timeout: product list section not found.");
        }

        await AutoScrollAsync(page);

        return await page.ContentAsync();
    }

    public static async Task SaveRenderedHtmlToFileAsync(string url, string filePath)
    {
        var html = await GetRenderedHtmlAsync(url);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, html, Encoding.UTF8);
        Console.WriteLine($"✅ Saved HTML to: {filePath}");
    }

    private static async Task AutoScrollAsync(IPage page)
    {
        await page.EvaluateAsync(@"async () => {
            await new Promise((resolve) => {
                let totalHeight = 0;
                const distance = 100;
                const timer = setInterval(() => {
                    const scrollHeight = document.body.scrollHeight;
                    window.scrollBy(0, distance);
                    totalHeight += distance;

                    if (totalHeight >= scrollHeight - window.innerHeight) {
                        clearInterval(timer);
                        resolve();
                    }
                }, 300);
            });
        }");
    }
}
