public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? Kcal { get; set; }

        public string? Category { get; set; } = string.Empty; 

public bool Discount { get; set; }

    public List<SupermarketProduct> SupermarketProducts { get; set; } = new();
}