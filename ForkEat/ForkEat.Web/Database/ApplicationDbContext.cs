using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}