using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

public class SklavenitisScraper : IScraper
{
    private readonly ILogger<SklavenitisScraper> _logger;
    private readonly GoogleSearchService _googleSearchService;

    private readonly List<string> _categoryUrls = new()
    {
    //    "https://www.sklavenitis.gr/eidi-artozacharoplasteioy/psomi-artoskeyasmata/" ,
    //     "https://www.sklavenitis.gr/eidi-artozacharoplasteioy/psomi-typopoiimeno/",
    //     "https://www.sklavenitis.gr/eidi-artozacharoplasteioy/pites-tortigies/" ,
    //    "https://www.sklavenitis.gr/eidi-artozacharoplasteioy/kritsinia-paximadia-fryganies/" ,
    //   "https://www.sklavenitis.gr/eidi-artozacharoplasteioy/koyloyria-voytimata/" ,
       "https://www.sklavenitis.gr/freska-froyta-lachanika/froyta/"
       // //,
    //    "https://www.sklavenitis.gr/freska-froyta-lachanika/lachanika/",
    //    "https://www.sklavenitis.gr/freska-froyta-lachanika/kommena-lahanika/" ,
    //     "https://www.sklavenitis.gr/fresko-psari-thalassina/psaria-ichthyokalliergeias/",
    //  "https://www.sklavenitis.gr/fresko-psari-thalassina/chtapodia-kalamaria-soypies/",
    //    "https://www.sklavenitis.gr/fresko-psari-thalassina/ostrakoeidi/" ,
    //    "https://www.sklavenitis.gr/fresko-kreas/fresko-moschari/",
    //    "https://www.sklavenitis.gr/fresko-kreas/fresko-choirino/",
    //    "https://www.sklavenitis.gr/fresko-kreas/freska-poylerika/",
    //    "https://www.sklavenitis.gr/fresko-kreas/freska-arnia-katsikia/",
    //    "https://www.sklavenitis.gr/fresko-kreas/freska-paraskeyasmata-kreaton-poylerikon/",
    //    "https://www.sklavenitis.gr/galata-rofimata-chymoi-psygeioy/galata-psygeioy/",
    //    "https://www.sklavenitis.gr/galata-rofimata-chymoi-psygeioy/galata-sokolatoycha-psygeioy/",
    //    "https://www.sklavenitis.gr/galata-rofimata-chymoi-psygeioy/futika-alla-rofimata-psugeiou/",
    //    "https://www.sklavenitis.gr/galata-rofimata-chymoi-psygeioy/chymoi-tsai-psygeioy/",
    //    "https://www.sklavenitis.gr/giaoyrtia-kremes-galaktos-epidorpia-psygeioy/giaoyrtia/",
    //    "https://www.sklavenitis.gr/giaoyrtia-kremes-galaktos-epidorpia-psygeioy/giaoyrtia-vrefika-paidika/",
    //    "https://www.sklavenitis.gr/giaoyrtia-kremes-galaktos-epidorpia-psygeioy/epidorpia-giaoyrtioy/",
    //    "https://www.sklavenitis.gr/giaoyrtia-kremes-galaktos-epidorpia-psygeioy/fytika-epidorpia/",
    //    "https://www.sklavenitis.gr/giaoyrtia-kremes-galaktos-epidorpia-psygeioy/ryzogala-glykismata-psygeioy/",
    //    "https://www.sklavenitis.gr/turokomika-futika-anapliromata/feta-leyka-tyria/",
    //    "https://www.sklavenitis.gr/turokomika-futika-anapliromata/malaka-tyria/",
    //    "https://www.sklavenitis.gr/turokomika-futika-anapliromata/imisklira-tyria/",
    //    "https://www.sklavenitis.gr/turokomika-futika-anapliromata/sklira-tyria/",
    //    "https://www.sklavenitis.gr/turokomika-futika-anapliromata/tyria-aleifomena-mini-tyrakia/",
    //    "https://www.sklavenitis.gr/turokomika-futika-anapliromata/futika-anapliromata/",
    //    "https://www.sklavenitis.gr/ayga-voytyro-nopes-zymes-zomoi/ayga/",
    //    "https://www.sklavenitis.gr/ayga-voytyro-nopes-zymes-zomoi/voytyra/",
    //    "https://www.sklavenitis.gr/ayga-voytyro-nopes-zymes-zomoi/margarines/",
    //    "https://www.sklavenitis.gr/ayga-voytyro-nopes-zymes-zomoi/zymes-nopes/",
    //    "https://www.sklavenitis.gr/ayga-voytyro-nopes-zymes-zomoi/zymarika-nopa/",
    //    "https://www.sklavenitis.gr/ayga-voytyro-nopes-zymes-zomoi/zomoi-psygeioy/",
    //    "https://www.sklavenitis.gr/allantika/allantika-galopoylas-kotopoyloy/",
    //    "https://www.sklavenitis.gr/allantika/zampon-mpeikon-omoplati/",
    //    "https://www.sklavenitis.gr/allantika/pariza-mortadela/",
    //    "https://www.sklavenitis.gr/allantika/salamia/",
    //    "https://www.sklavenitis.gr/allantika/loykanika/",
    //    "https://www.sklavenitis.gr/allantika/paradosiaka-allantika/",
    //    "https://www.sklavenitis.gr/allantika/set-allantikon-tyrion/",
    //    "https://www.sklavenitis.gr/allantika/anapliromata-allantikon/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/psaria-pasta-se-ladi/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/kapnista-psaria/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/delicatessen-thalassinon/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/pate-foie-gras/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/salates-aloifes/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/elies/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/toyrsia-liastes-tomates/",
    //    "https://www.sklavenitis.gr/orektika-delicatessen/chalvades/",
    //    "https://www.sklavenitis.gr/etoima-geymata/geymata-me-kreas-poylerika/",
    //    "https://www.sklavenitis.gr/etoima-geymata/geymata-me-psaria-thalassina-sushi/",
    //    "https://www.sklavenitis.gr/etoima-geymata/geymata-osprion-lachanikon/",
    //    "https://www.sklavenitis.gr/etoima-geymata/ladera/",
    //    "https://www.sklavenitis.gr/etoima-geymata/geymata-zymarikon-ryzioy/",
    //    "https://www.sklavenitis.gr/etoima-geymata/soupes/",
    //    "https://www.sklavenitis.gr/etoima-geymata/etoimes-salates-synodeytika-geymaton/",
    //    "https://www.sklavenitis.gr/etoima-geymata/santoyits/",
    //    "https://www.sklavenitis.gr/katepsygmena/katepsygmena-psaria-thalassina/",
    //    "https://www.sklavenitis.gr/katepsygmena/katepsygmena-kreata-poylerika/",       
    //    "https://www.sklavenitis.gr/katepsygmena/katepsygmena-fytika-anapliromata/",
    //    "https://www.sklavenitis.gr/katepsygmena/katepsygmena-geymata/",
    //    "https://www.sklavenitis.gr/eidi-proinoy-rofimata/kafedes-rofimata-afepsimata/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/aleyri-simigdali/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/zymarika/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/ketsap-moystardes-magionezes-etoimes-saltses/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/mpacharika-alatia-xidia-zomoi/",
    //     "https://www.sklavenitis.gr/trofima-pantopoleioy/ryzia/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/ospria/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/sitari-kinoa-sogia-alla-dimitriaka/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/poyredes-soypes-noodles/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/ntomatika/",
    //    "https://www.sklavenitis.gr/trofima-pantopoleioy/meigmata-gia-zele-glyka/",
    };

    public SklavenitisScraper(ILogger<SklavenitisScraper> logger, GoogleSearchService googleSearchService)
    {
        _logger = logger;
        _googleSearchService = googleSearchService;
    }

    public async Task<List<Product>> ScrapeProductsAsync(string query)
    {
        var allProducts = new List<Product>();

        foreach (var categoryUrl in _categoryUrls)
        {
            _logger.LogInformation($"🔎 Scraping category URL: {categoryUrl}");
            var categoryProducts = await ScrapeCategoryAsync(categoryUrl);
            allProducts.AddRange(categoryProducts);
        }

        _logger.LogInformation($"✅ Scraping complete. Total products scraped: {allProducts.Count}");
        return allProducts;
    }

    private async Task<List<Product>> ScrapeCategoryAsync(string categoryUrl)
    {
        var products = new List<Product>();
        string html;

        try
        {
            html = await SklavenitisPlaywrightHelper.GetRenderedHtmlAsync(categoryUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Failed to render page: {ex.Message}");
            return products;
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var productNodes = htmlDoc.DocumentNode.SelectNodes("//section[contains(@class, 'productList')]//div[contains(@class, 'product_innerTop')]");
        if (productNodes == null)
        {
            _logger.LogWarning($"⚠️ No products found for category URL: {categoryUrl}");
            return products;
        }

        string categoryName = ExtractCategoryFromUrl(categoryUrl);

        foreach (var topNode in productNodes)
        {
            try
            {
                var container = topNode.ParentNode; // container that includes pricing
                var imgNode = topNode.SelectSingleNode(".//img[@title]");
                var name = imgNode?.GetAttributeValue("title", null)?.Trim();

                var priceNode = container.SelectSingleNode(".//div[contains(@class,'product_prices')]//div[contains(@class,'price') and @data-price]");
                var priceText = priceNode?.GetAttributeValue("data-price", null)?.Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceText))
                {
                    _logger.LogWarning("⚠️ Skipped product due to missing name or price.");
                    continue;
                }

                priceText = Regex.Replace(priceText, @"[^\d,\.]", "").Replace(",", ".");
                if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                {
                    _logger.LogWarning($"⚠️ Could not parse price for '{name}'. Raw: '{priceText}'");
                    continue;
                }

                var product = new Product
                {
                    Name = name,
                    Price = price,
                    Category = categoryName,
                    Discount = false // Sklavenitis doesn’t expose this directly
                };

                products.Add(product);
                _logger.LogInformation($"✅ Parsed: '{product.Name}' | {product.Category} | {product.Price}€");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error parsing product node: {ex.Message}");
            }
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
}
