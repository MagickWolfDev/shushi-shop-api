using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shushi_shop_api.Models;

namespace shushi_shop_api.Data
{
    public class shushi_shop_apiContext : DbContext
    {
        public shushi_shop_apiContext (DbContextOptions<shushi_shop_apiContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<shushi_shop_api.Models.User> User { get; set; } = default!;

        public DbSet<shushi_shop_api.Models.Dish> Dish { get; set; } = default!;

        public DbSet<shushi_shop_api.Models.ProductType> ProductType { get; set; } = default!;
    }
}
