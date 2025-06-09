public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task<List<Supermarket>> GetSupermarketsByProductAsync(int productId);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);

    Task<Product?> GetByNameAsync(string name);
Task<(List<Product> Items, int TotalCount)> SearchAsync(
    string? name,
    decimal? minPrice,
    decimal? maxPrice,
    int? minKcal,
    int? maxKcal,
    string? sortBy,
    string? sortOrder,
    int page,
    int pageSize);
}
