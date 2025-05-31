using Microsoft.EntityFrameworkCore;
  
public class ProductRepository : IProductRepository
{
    private readonly BudgetDbContext _context;

    public ProductRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.SupermarketProducts)
            .ThenInclude(sp => sp.Supermarket)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Supermarket>> GetSupermarketsByProductAsync(int productId)
    {
        return await _context.SupermarketProducts
            .Where(sp => sp.ProductId == productId)
            .Select(sp => sp.Supermarket)
            .ToListAsync();
    }
}
