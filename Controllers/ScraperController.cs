
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class ScrapeController : ControllerBase
{
    private readonly Func<string, IScraper> _scraperFactory;
    private readonly ISupermarketRepository _smRepo;
    private readonly IProductRepository _productRepo;

    private readonly ILogger<ScrapeController> _logger;


    public ScrapeController(
        ILogger<ScrapeController> logger,
        Func<string, IScraper> scraperFactory,
        ISupermarketRepository smRepo,
        IProductRepository productRepo)
    {
        _scraperFactory = scraperFactory;
        _smRepo = smRepo;
        _productRepo = productRepo;
        _logger = logger;
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

        var products = await scraper.ScrapeProductsAsync(request.Query ?? normalized);

        int added = 0, updated = 0, skipped = 0, linked = 0;

        // Log all scraped products
        foreach (var product in products)
        {
            _logger.LogInformation($"{product.Category} - {product.Name}: {product.Price}€");
        }

        // Ensure supermarket exists
        var supermarket = await _smRepo.GetByNameAsync(normalized);
        if (supermarket == null)
        {
            supermarket = await _smRepo.AddAsync(new Supermarket { Name = normalized });
        }

        // Get already linked product IDs for the supermarket
        var alreadyLinked = await _smRepo.GetLinkedProductIds(supermarket.Id);

        // Process each product
        foreach (var p in products)
        {
            if (p.Price <= 0)
            {
                _logger.LogWarning($"⚠️ Skipping '{p.Name}' due to invalid price ({p.Price}€).");
                skipped++;
                continue;
            }

            var normalizedName = p.Name.Trim().ToLower();
            var existing = await _productRepo.GetByNameAsync(normalizedName);

            if (existing == null)
            {
                await _productRepo.AddAsync(p);
                existing = p;
                added++;
            }
            else
            {
                if (alreadyLinked.Contains(existing.Id) && existing.Price != p.Price)
                {
                    var oldPrice = existing.Price;
                    existing.Price = p.Price;
                    await _productRepo.UpdateAsync(existing);
                    updated++;
                    _logger.LogInformation($"🔄 Updated price for '{existing.Name}' (supermarket '{supermarket.Name}'): {oldPrice}€ → {p.Price}€");
                }
            }

            if (!alreadyLinked.Contains(existing.Id))
            {
                await _smRepo.AddProductToSupermarketAsync(supermarket.Id, existing.Id);
                linked++;
            }
        }

        _logger.LogInformation($"✅ Summary: Added={added}, Updated={updated}, Linked={linked}, Skipped={skipped}");

        return Ok(products);
    }
}