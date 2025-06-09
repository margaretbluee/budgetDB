
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class ScrapeController : ControllerBase
{
    private readonly Func<string, IScraper> _scraperFactory;
    private readonly ISupermarketRepository _smRepo;
    private readonly IProductRepository _productRepo;

    public ScrapeController(
        Func<string, IScraper> scraperFactory,
        ISupermarketRepository smRepo,
        IProductRepository productRepo)
    {
        _scraperFactory = scraperFactory;
        _smRepo = smRepo;
        _productRepo = productRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Scrape([FromBody] ScrapeRequest request)
    {
        var normalized = request.Supermarket.ToLowerInvariant();

        IScraper scraper;
        try
        {
            scraper = _scraperFactory(normalized);
        }
        catch (KeyNotFoundException)
        {
            return BadRequest($"Unsupported supermarket scraper: {normalized}");
        }

        // var products = await scraper.ScrapeProductsAsync(request.Query ?? normalized);
    var products = await scraper.ScrapeProductsAsync(request.Query ?? normalized);

foreach (var product in products)
{
    Console.WriteLine($"{product.Category} - {product.Name}: {product.Price}€");
}
        var supermarket = await _smRepo.GetByNameAsync(normalized);
        if (supermarket == null)
        {
            supermarket = await _smRepo.AddAsync(new Supermarket { Name = normalized });
        }

        var alreadyLinked = await _smRepo.GetLinkedProductIds(supermarket.Id);

        foreach (var p in products)
        {
            var normalizedName = p.Name.Trim().ToLower();
            var existing = await _productRepo.GetByNameAsync(normalizedName);
            if (existing == null)
            {
                await _productRepo.AddAsync(p);
                existing = p;
            }

            if (!alreadyLinked.Contains(existing.Id))
            {
                await _smRepo.AddProductToSupermarketAsync(supermarket.Id, existing.Id);
            }
        }

        return Ok(products);
    }
}
