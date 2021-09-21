using ForkEat.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(user => user.Id);
            modelBuilder.Entity<User>().Property(user => user.Email);
            modelBuilder.Entity<User>().Property(user => user.Password);
            modelBuilder.Entity<User>().Property(user => user.UserName);

            modelBuilder.Entity<Product>().HasKey(product => product.Id);
            modelBuilder.Entity<Product>().Property(product => product.Name);
        }
    }
}