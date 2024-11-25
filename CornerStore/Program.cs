using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using CornerStore.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core and provides dummy value for testing
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//endpoints go here
app.MapGet("/cashiers", (CornerStoreDbContext db) => 
{
    return db.Cashiers.Select(c => new CashierDTO
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Orders = c.Orders.Select(o => new OrderDTO 
        {
            Id = o.Id,
            CashierId = o.CashierId,
            PaidOnDate = o.PaidOnDate,
            OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
            {
                OrderId = op.OrderId,
                ProductId = op.ProductId,
                Quantity = op.Quantity,
                Product = new ProductDTO 
                {
                    Id = op.Product.Id, 
                    Brand = op.Product.Brand, 
                    CategoryId = op.Product.CategoryId,
                    Price = op.Product.Price,
                    Category = new CategoryDTO {Id = op.Product.Category.Id, CategoryName = op.Product.Category.CategoryName}
                }
            }).ToList()
        }).ToList()
    });   
});

app.MapPost("/cashiers", (CornerStoreDbContext db, Cashier cashier) => 
{
    db.Cashiers.Add(cashier);
    db.SaveChanges();

    return Results.Created($"/cashiers/{cashier.Id}", new CashierDTO
    {
        Id= cashier.Id,
        FirstName = cashier.FirstName,
        LastName = cashier.LastName   
    });
});

app.MapGet("/products", (CornerStoreDbContext db, string? search ) => 
{
    List<ProductDTO> products = db.Products.Select(p => new ProductDTO
    {
        Id = p.Id,
        ProductName = p.ProductName,
        Price = p.Price,
        Brand = p.Brand,
        CategoryId = p.CategoryId,
        Category = new CategoryDTO 
        {
            Id = p.Category.Id,
            CategoryName = p.Category.CategoryName
        }
    }).ToList();

    if (search != null)
    {
        products = products.Where(p => p.ProductName.ToLower() == search.ToLower() || p.Category.CategoryName.ToLower() == search.ToLower()).ToList();
    }

    return products;
});

app.MapPost("/Products", (CornerStoreDbContext db, Product product) => 
{
    db.Products.Add(product);
    db.SaveChanges();
});

app.MapPut("/products/{id}", (CornerStoreDbContext db, Product product, int id) => 
{
    try
    {
        Product foundProduct = db.Products.Single(p => p.Id == product.Id && id == p.Id);
        foundProduct = product;
        db.SaveChanges();

        return Results.Accepted();
        
    }
    catch (System.Exception)
    {
        
        return Results.BadRequest();
    }
});

app.MapGet("/products/popular", async (CornerStoreDbContext db, int? amount) => 
{
    var orderProducts = await db.OrderProducts
    .Include(p => p.Product) 
    .GroupBy(p => new { p.ProductId, p.Product.ProductName }) 
    .Select(group => new 
    {
        TheProductName = group.Key.ProductName, 
        ProductId = group.Key.ProductId,      
        TotalQuantity = group.Sum(p => p.Quantity)
    })
    .ToListAsync();
    
    
    if (amount != null)
    {
        
     orderProducts = orderProducts.Take(amount.Value).ToList();

    }

    return orderProducts;
});

app.MapGet("/orders", (CornerStoreDbContext db, string? orderDate ) => 
{
    List<OrderDTO> orders = db.Orders.Select(o => new OrderDTO
    {
        Id = o.Id,
        CashierId = o.CashierId,
        Cashier = new CashierDTO{Id = o.Cashier.Id, FirstName = o.Cashier.FirstName, LastName = o.Cashier.LastName},
        PaidOnDate = o.PaidOnDate,
        OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
        {
            OrderId = op.OrderId,
            ProductId = op.ProductId,
            Quantity = op.Quantity,
            Product = new ProductDTO {Id = op.Product.Id, Price = op.Product.Price, ProductName = op.Product.ProductName}

        }).ToList()
    }).ToList();

    if (orderDate != null)
    {
        
        orders = orders.Where(o => o.PaidOnDate.HasValue && o.PaidOnDate?.ToString("yyyy-MM-dd") == orderDate ).ToList();
    }
    return orders;

});

app.MapGet("/orders/{id}", (CornerStoreDbContext db, int id ) => 
{
    return db.Orders.Select(o => new OrderDTO
    {
        Id = o.Id,
        CashierId = o.CashierId,
        Cashier = new CashierDTO{Id = o.Cashier.Id, FirstName = o.Cashier.FirstName, LastName = o.Cashier.LastName},
        PaidOnDate = o.PaidOnDate,
        OrderProducts = o.OrderProducts.Select(op => new OrderProductDTO
        {
            OrderId = op.OrderId,
            ProductId = op.ProductId,
            Quantity = op.Quantity,
            Product = new ProductDTO 
            {
                Id = op.Product.Id, 
                Price = op.Product.Price, 
                ProductName = op.Product.ProductName,
                CategoryId = op.Product.CategoryId,
                Category = new CategoryDTO {Id = op.Product.Category.Id, CategoryName = op.Product.Category.CategoryName}
                
            }

        }).ToList()
    }).Single(o => o.Id == id);

    
    

});

app.MapPost("/orders", (CornerStoreDbContext db, Order order) => 
{
    db.Orders.Add(order);
    db.SaveChanges();
});

app.MapDelete("/orders/{id}", (CornerStoreDbContext db, int id) => 
{
    Order order = db.Orders.Single(o => o.Id == id);
    db.Orders.Remove(order);
    db.SaveChanges();
});

app.Run();

//don't move or change this!
public partial class Program { }