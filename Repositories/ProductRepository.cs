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

    // public async Task AddAsync(Product product)
    // {
    //     _context.Products.Add(product);
    //     await _context.SaveChangesAsync();
    // }

    public async Task AddAsync(Product product)
{
    try
    {
        _context.Products.Add(product);
        var result = await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Saved {result} record(s) to DB: {product.Name}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error saving product '{product.Name}': {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
    }
}


    public async Task<List<Supermarket>> GetSupermarketsByProductAsync(int productId)
    {
        return await _context.SupermarketProducts
            .Where(sp => sp.ProductId == productId)
            .Select(sp => sp.Supermarket)
            .ToListAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<Product> Items, int TotalCount)> SearchAsync(
       string? name,
       decimal? minPrice,
       decimal? maxPrice,
       int? minKcal,
       int? maxKcal,
       string? sortBy,
       string? sortOrder,
       int page,
       int pageSize)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (minKcal.HasValue)
            query = query.Where(p => p.Kcal >= minKcal.Value);

        if (maxKcal.HasValue)
            query = query.Where(p => p.Kcal <= maxKcal.Value);

        // Total before pagination
        int totalCount = await query.CountAsync();

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool descending = sortOrder?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "kcal" => descending ? query.OrderByDescending(p => p.Kcal) : query.OrderBy(p => p.Kcal),
                _ => query
            };
        }

        // Paging
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }
public Task<Product?> GetByNameAsync(string name)
{
    var normalized = name.Trim().ToLower();
    return _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == normalized);
}
}
