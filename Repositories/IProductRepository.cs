public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task<List<Supermarket>> GetSupermarketsByProductAsync(int productId);
}
