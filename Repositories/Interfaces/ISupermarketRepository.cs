public interface ISupermarketRepository
{
    Task<List<Supermarket>> GetAllAsync();
    Task<Supermarket?> GetByIdAsync(int id);
    Task<Supermarket> AddAsync(Supermarket supermarket); // ✅ Add correct return type
    Task<Supermarket?> GetByNameAsync(string name);       // ✅ Add this method
    Task AddProductToSupermarketAsync(int supermarketId, int productId);
    Task<List<Product>> GetProductsBySupermarketAsync(int supermarketId);
    Task UpdateAsync(Supermarket supermarket);
    Task DeleteAsync(int id);
     Task<List<int>> GetLinkedProductIds(int supermarketId);
    Task<List<Supermarket>> SearchByNameAsync(string? name);
}