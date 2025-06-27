using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

public class MasoutisScraper : IScraper
{
    private readonly ILogger<MasoutisScraper> _logger;
    private readonly GoogleSearchService _googleSearchService;
    private readonly List<string> _categoryUrls = new()
    {
       "https://www.masoutis.gr/categories/index/kreopwleio?item=565" ,
        "https://www.masoutis.gr/categories/index/manabiko?item=566",
        "https://www.masoutis.gr/categories/index/eidh-psugeiou?item=568" ,
       "https://www.masoutis.gr/categories/index/eidh-katapsukshs?item=573" ,
      "https://www.masoutis.gr/categories/index/ugieinh-diatrofh?item=564" ,
       "https://www.masoutis.gr/categories/index/biologika-proionta?item=5",
       "https://www.masoutis.gr/categories/index/vegan-proionta?item=6",
       "https://www.masoutis.gr/categories/index/proionta-xwris-gloutenh?item=7" ,
        "https://www.masoutis.gr/categories/index/diethnhs-kouzina?item=10",
     "https://www.masoutis.gr/categories/index/artozaxaroplasteio?item=575",
       "https://www.masoutis.gr/categories/index/eidh-pantopwleiou?item=562" ,
       "https://www.masoutis.gr/categories/index/zumarika-ospria?item=577",
       "https://www.masoutis.gr/categories/index/konserboeidh?item=578" 
 
    };

    public MasoutisScraper(ILogger<MasoutisScraper> logger, GoogleSearchService googleSearchService)
    {
        _logger = logger;
        _googleSearchService = googleSearchService;
    }
private async Task<List<Product>> ScrapeCategoryAsync(string categoryUrl)
{
    _logger.LogInformation($"Starting scrape for category URL: {categoryUrl}");

    var products = new List<Product>();
    string html;

    try
    {
        html = await MasoutisPlaywrightHelper.GetRenderedHtmlAsync(categoryUrl);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Failed to render page: {ex.Message}");
        return products;
    }

    var htmlDoc = new HtmlDocument();
    htmlDoc.LoadHtml(html);

    var productNodes = htmlDoc.DocumentNode
        .SelectNodes("//div[contains(@class, 'productList')]/div[contains(@class, 'product')]");

    if (productNodes == null)
    {
        _logger.LogWarning($"No products found for category URL: {categoryUrl}");
        return products;
    }

    string categoryName = ExtractCategoryFromUrl(categoryUrl);

foreach (var node in productNodes)
{
    var imgNode = node.SelectSingleNode(".//img[contains(@class, 'productImage')]");
    var priceNode = node.SelectSingleNode(".//span[contains(@class, 'extracted-price')]");
    var discountNode = node.SelectSingleNode(".//span[contains(@class, 'discount-flag')]");

    string name = imgNode?.GetAttributeValue("title", "Unknown")?.Trim() ?? "Unknown";
    string priceText = priceNode?.InnerText?.Trim() ?? "0";
    bool discount = discountNode?.InnerText.Trim().ToLower() == "true";

    priceText = Regex.Replace(priceText, @"[^\d,\.]", "").Replace(",", ".");

    if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
    {
        _logger.LogWarning($"⚠️ Could not parse price for '{name}' in category '{categoryName}'. Raw text: '{priceText}'");
        continue;
    }

    var product = new Product
    {
        Name = name,
        Price = price,
        Category = categoryName,
        Discount = discount
    };

    products.Add(product);
    _logger.LogInformation($"✅ Parsed: '{product.Name}' | {product.Category} | {product.Price}€ | Discount: {product.Discount}");
}


    return products;
}


    private string ExtractCategoryFromUrl(string url)
    {
        try
        {
            var parts = url.Split("/", StringSplitOptions.RemoveEmptyEntries);
            return parts.LastOrDefault()?.Replace("-", " ") ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

public async Task<List<Product>> ScrapeProductsAsync(string query)
{
    var allProducts = new List<Product>();

    foreach (var categoryUrl in _categoryUrls)
    {
        _logger.LogInformation($"Scraping category URL: {categoryUrl}");
        
        var categoryProducts = await ScrapeCategoryAsync(categoryUrl);
        allProducts.AddRange(categoryProducts);
    }

    _logger.LogInformation($"Scraping complete. Total products scraped: {allProducts.Count}");

    return allProducts;
}

}
