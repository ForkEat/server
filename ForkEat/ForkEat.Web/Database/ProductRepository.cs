using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;

namespace ForkEat.Web.Database
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task<Product> InsertProduct(Product product)
        {
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }
    }
}