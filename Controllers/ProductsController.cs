using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepo;

    public ProductsController(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _productRepo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await _productRepo.AddAsync(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    // [HttpPost]
    // public async Task<IActionResult> AddProduct(Product product)
    // {
    //     await _productRepo.AddAsync(product);
    //     return Ok(product);
    // }

    [HttpGet("{productId}/supermarkets")]
    public async Task<IActionResult> GetSupermarketsByProduct(int productId)
    {
        var supermarkets = await _productRepo.GetSupermarketsByProductAsync(productId);
        return Ok(supermarkets);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest("ID mismatch");

        var existing = await _productRepo.GetByIdAsync(id);
        if (existing == null)
            return NotFound("Product not found.");


        await _productRepo.UpdateAsync(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _productRepo.GetByIdAsync(id);
        if (existing == null)
            return NotFound("Product not found.");

        await _productRepo.DeleteAsync(id);
        return NoContent();
    }

[HttpGet("search")]
public async Task<IActionResult> Search(
    [FromQuery] string? name,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] int? minKcal,
    [FromQuery] int? maxKcal,
    [FromQuery] string? sortBy,
    [FromQuery] string? sortOrder = "asc",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    var (items, totalCount) = await _productRepo.SearchAsync(name, minPrice, maxPrice, minKcal, maxKcal, sortBy, sortOrder, page, pageSize);

    return Ok(new
    {
        totalCount,
        page,
        pageSize,
        items
    });
}
}