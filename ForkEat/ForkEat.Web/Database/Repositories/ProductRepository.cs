using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public Task<Product> FindProductById(Guid id)
        {
            return dbContext
                .Products
                .FirstOrDefaultAsync(product => product.Id == id);
        }

        public Task<List<Product>> FindAllProducts()
        {
            return dbContext.Products.ToListAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Product> UpdateProduct(Product newProduct)
        {
            dbContext.Products.Update(newProduct);
            await dbContext.SaveChangesAsync();
            return await FindProductById(newProduct.Id);
        }

        public Task<Dictionary<Guid, Product>> FindProductsByIds(List<Guid> productIds)
        {
            throw new NotImplementedException();
        }
    }
}