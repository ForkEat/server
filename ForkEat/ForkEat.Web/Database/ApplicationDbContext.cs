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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(user => user.Id);
            modelBuilder.Entity<User>().Property(user => user.Email);
            modelBuilder.Entity<User>().Property(user => user.Password);
            modelBuilder.Entity<User>().Property(user => user.UserName);
        }
    }
}