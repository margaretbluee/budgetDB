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

    public async Task<Supermarket> AddAsync(Supermarket supermarket)
    {
        _context.Supermarkets.Add(supermarket);
        await _context.SaveChangesAsync();
        return supermarket;
    }

    // public async Task AddProductToSupermarketAsync(int supermarketId, int productId)
    // {
    //     var entry = new SupermarketProduct
    //     {
    //         SupermarketId = supermarketId,
    //         ProductId = productId
    //     };
    //     _context.SupermarketProducts.Add(entry);
    //     await _context.SaveChangesAsync();
    // }

    public async Task AddProductToSupermarketAsync(int supermarketId, int productId)
{
    bool alreadyExists = await _context.SupermarketProducts
        .AnyAsync(sp => sp.SupermarketId == supermarketId && sp.ProductId == productId);

   if (!alreadyExists)
{
    _context.SupermarketProducts.Add(new SupermarketProduct
    {
        SupermarketId = supermarketId,
        ProductId = productId
    });
    await _context.SaveChangesAsync();
    Console.WriteLine($"✅ Linked Product {productId} to Supermarket {supermarketId}");
}
else
{
    Console.WriteLine($"⚠️ Product {productId} already linked to Supermarket {supermarketId}");
}
}

    public async Task<List<Product>> GetProductsBySupermarketAsync(int supermarketId)
    {
        return await _context.SupermarketProducts
            .Where(sp => sp.SupermarketId == supermarketId)
            .Select(sp => sp.Product)
            .ToListAsync();
    }
    public async Task UpdateAsync(Supermarket supermarket)
    {
        _context.Supermarkets.Update(supermarket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supermarket = await _context.Supermarkets.FindAsync(id);
        if (supermarket != null)
        {
            _context.Supermarkets.Remove(supermarket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<int>> GetLinkedProductIds(int supermarketId)
{
    return await _context.SupermarketProducts
        .Where(sp => sp.SupermarketId == supermarketId)
        .Select(sp => sp.ProductId)
        .ToListAsync();
}

    public async Task<List<Supermarket>> SearchByNameAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await _context.Supermarkets.ToListAsync();

        return await _context.Supermarkets
            .Where(s => s.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<Supermarket?> GetByNameAsync(string name)
{
    return await _context.Supermarkets
        .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
}
}
