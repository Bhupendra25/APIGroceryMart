
using GroceryStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Data
{
    public class GroceryContext : IdentityDbContext<IdentityUser>
    {
        public GroceryContext(DbContextOptions<GroceryContext> options) : base(options)
        {
        }

        public DbSet<Admin> admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}