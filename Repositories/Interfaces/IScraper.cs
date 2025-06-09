public interface IScraper
{
    Task<List<Product>> ScrapeProductsAsync(string query);
}