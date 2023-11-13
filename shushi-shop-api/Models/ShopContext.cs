using Microsoft.EntityFrameworkCore;
using shushi_shop_api.Models;
using System.Collections.Generic;

namespace shushi_shop_api.Models;

public class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options)
    : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Dish> Dishes { get; set; } = null!;
    public DbSet<ProductType> ProductTypes { get; set; } = null!;

}