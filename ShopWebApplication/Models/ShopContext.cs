using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApplication.Models;

public class ShopContext : IdentityDbContext<User>
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options) {}
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<ClientInfo> ClientInfos { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }
    public virtual DbSet<Status> Statuses { get; set; }
    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);
        
        modelBuilder.Entity<Cart>()
            .Property(c => c.TotalPrice)
            .HasColumnType("decimal(18, 2)");
    }
}
