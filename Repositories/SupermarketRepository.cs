using Microsoft.EntityFrameworkCore;

public class SupermarketRepository : ISupermarketRepository
{
    private readonly BudgetDbContext _context;

    public SupermarketRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supermarket>> GetAllAsync()
    {
        return await _context.Supermarkets.ToListAsync();
    }

    public async Task<Supermarket?> GetByIdAsync(int id)
    {
        return await _context.Supermarkets
            .Include(s => s.SupermarketProducts)
            .ThenInclude(sp => sp.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(Supermarket supermarket)
    {
        _context.Supermarkets.Add(supermarket);
        await _context.SaveChangesAsync();
    }

    public async Task AddProductToSupermarketAsync(int supermarketId, int productId)
    {
        var entry = new SupermarketProduct
        {
            SupermarketId = supermarketId,
            ProductId = productId
        };
        _context.SupermarketProducts.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetProductsBySupermarketAsync(int supermarketId)
    {
        return await _context.SupermarketProducts
            .Where(sp => sp.SupermarketId == supermarketId)
            .Select(sp => sp.Product)
            .ToListAsync();
    }
}
