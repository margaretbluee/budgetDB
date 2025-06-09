public class Supermarket
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;


    public List<SupermarketProduct> SupermarketProducts { get; set; } = new();
}