using Microsoft.Playwright;
using System.Text;

public class MasoutisPlaywrightHelper
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

    await page.WaitForSelectorAsync("div.productList", new PageWaitForSelectorOptions
    {
        Timeout = 30000
    });

    // scroll logic to trigger lazy loading
    await AutoScrollAsync(page);

    // inject price spans as before
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

// helper method to scroll until bottom
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
            }, 300); // slow enough to allow lazy loading
        });
    }");
}

}
