
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SupermarketsController : ControllerBase
{
    private readonly ISupermarketRepository _repo;

    public SupermarketsController(ISupermarketRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Supermarket s)
    {
        await _repo.AddAsync(s);
        return CreatedAtAction(nameof(GetAll), new { id = s.Id }, s);
    }

    [HttpPost("{supermarketId}/add-product/{productId}")]
    public async Task<IActionResult> AddProductToSupermarket(int supermarketId, int productId)
    {
        await _repo.AddProductToSupermarketAsync(supermarketId, productId);
        return Ok("Product added to supermarket.");
    }

    [HttpGet("{supermarketId}/products")]
    public async Task<IActionResult> GetProductsBySupermarket(int supermarketId)
    {
        var products = await _repo.GetProductsBySupermarketAsync(supermarketId);
        return Ok(products);
    }
}

