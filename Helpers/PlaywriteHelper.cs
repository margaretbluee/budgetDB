using Microsoft.Playwright;
using System.Text;

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

        // Wait for product list to load
        await page.WaitForSelectorAsync("div.productList", new PageWaitForSelectorOptions
        {
            Timeout = 30000
        });

        // Run JS in browser context to insert consistent price spans into DOM
    await page.EvaluateAsync(@"
    Array.from(document.querySelectorAll('div.productList .product')).forEach(product => {
        let hasRegularPrice = !!product.querySelector('div.price');
        let regularPrice = product.querySelector('div.price')?.innerText;
        let discountPrice = product.querySelector('div.disPrices-wrapper div.pDscntPrice.t-right')?.innerText;
        
        let finalPrice = regularPrice || discountPrice;
        if (finalPrice) {
            let priceSpan = document.createElement('span');
            priceSpan.className = 'extracted-price';
            priceSpan.textContent = finalPrice;
            product.appendChild(priceSpan);

            let discountSpan = document.createElement('span');
            discountSpan.className = 'discount-flag';
            discountSpan.textContent = hasRegularPrice ? 'false' : 'true';
            product.appendChild(discountSpan);
        }
    });
");

        return await page.ContentAsync();
    }
}
