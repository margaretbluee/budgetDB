using Microsoft.EntityFrameworkCore;

public class BudgetDbContext : DbContext
{
    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) {}

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Supermarket> Supermarkets => Set<Supermarket>();
    public DbSet<SupermarketProduct> SupermarketProducts => Set<SupermarketProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SupermarketProduct>()
            .HasKey(sp => new { sp.SupermarketId, sp.ProductId });

        modelBuilder.Entity<SupermarketProduct>()
            .HasOne(sp => sp.Supermarket)
            .WithMany(s => s.SupermarketProducts)
            .HasForeignKey(sp => sp.SupermarketId);

        modelBuilder.Entity<SupermarketProduct>()
            .HasOne(sp => sp.Product)
            .WithMany(p => p.SupermarketProducts)
            .HasForeignKey(sp => sp.ProductId);
    }
}
