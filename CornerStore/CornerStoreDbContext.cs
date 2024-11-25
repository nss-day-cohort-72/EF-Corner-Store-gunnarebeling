using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{
    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet <OrderProduct> OrderProducts {get; set;}

    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {

    }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Cashier>().HasData(new Cashier[]
        {
            new Cashier { Id = 1, FirstName = "Alice", LastName = "Johnson" },
            new Cashier { Id = 2, FirstName = "Bob", LastName = "Smith" },
            new Cashier { Id = 3, FirstName = "Charlie", LastName = "Brown" },
            new Cashier { Id = 4, FirstName = "Diana", LastName = "Prince" },
            new Cashier { Id = 5, FirstName = "Ethan", LastName = "Hawke" }
        });
        modelBuilder.Entity<Category>().HasData(new Category[]
        {
            new Category { Id = 1, CategoryName = "Electronics" },
            new Category { Id = 2, CategoryName = "Clothing" },
            new Category { Id = 3, CategoryName = "Books" },
            new Category { Id = 4, CategoryName = "Toys" }
        });

        modelBuilder.Entity<Order>().HasData(new Order[]
        {
            new Order { Id = 1, CashierId = 1 },
            new Order { Id = 2, CashierId = 2 },
            new Order { Id = 3, CashierId = 3 },
            new Order { Id = 4, CashierId = 4 }
        });

        modelBuilder.Entity<Product>().HasData(new Product[]
        {
            new Product { Id = 1, ProductName = "Laptop", Price = 999.99m, Brand = "Dell", CategoryId = 1 },
            new Product { Id = 2, ProductName = "T-Shirt", Price = 19.99m, Brand = "Nike", CategoryId = 2 },
            new Product { Id = 3, ProductName = "Novel", Price = 14.99m, Brand = "Penguin", CategoryId = 3 },
            new Product { Id = 4, ProductName = "Toy Car", Price = 9.99m, Brand = "Hot Wheels", CategoryId = 4 }

        });
        modelBuilder.Entity<OrderProduct>()
        .HasKey(op => new { op.OrderId, op.ProductId });

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId);

        modelBuilder.Entity<OrderProduct>().HasData(new OrderProduct[]
        {
            new OrderProduct { OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrderProduct { OrderId = 1, ProductId = 2, Quantity = 1 },
            new OrderProduct { OrderId = 2, ProductId = 3, Quantity = 3 },
            new OrderProduct { OrderId = 3, ProductId = 4, Quantity = 5 }
        });
        
        


    }

}