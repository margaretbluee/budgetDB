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

    [HttpPost]
    public async Task<IActionResult> AddProduct(Product product)
    {
        await _productRepo.AddAsync(product);
        return Ok(product);
    }

    [HttpGet("{productId}/supermarkets")]
    public async Task<IActionResult> GetSupermarketsByProduct(int productId)
    {
        var supermarkets = await _productRepo.GetSupermarketsByProductAsync(productId);
        return Ok(supermarkets);
    }
}