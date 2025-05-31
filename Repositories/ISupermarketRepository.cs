public interface ISupermarketRepository
{
    Task<List<Supermarket>> GetAllAsync();
    Task<Supermarket?> GetByIdAsync(int id);
    Task AddAsync(Supermarket supermarket);
    Task AddProductToSupermarketAsync(int supermarketId, int productId);
    Task<List<Product>> GetProductsBySupermarketAsync(int supermarketId);
}
