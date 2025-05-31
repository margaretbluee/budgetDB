public class SupermarketProduct
{
    public int SupermarketId { get; set; }
    public Supermarket Supermarket { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}