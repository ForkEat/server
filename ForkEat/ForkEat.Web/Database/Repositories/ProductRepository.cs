using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Product> InsertProduct(Product product)
    {
        await dbContext.Products.AddAsync(new ProductEntity(product));
        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<Product> FindProductById(Guid id)
    {
        var entity = await dbContext
            .Products
            .FirstOrDefaultAsync(product => product.Id == id);

        return entity is null ? null : new Product(entity.Id, entity.Name, entity.ImageId, entity.ProductType);
    }

        public async Task<List<Guid>> FindProductIdWithFullTextSearch(string[] words)
        {
            var products = await dbContext.Products.ToListAsync();
            return products
                .Where(p => words.Any(w => p.Name.ToLower().Contains(w.ToLower())))
                .Select(p => p.Id)
                .ToList();
        }

        public async Task<List<Product>> FindAllProducts()
        {
            var products = await dbContext.Products.ToListAsync();
            return products
                .Select(entity => new Product(entity.Id, entity.Name, entity.ImageId, entity.ProductType))
                .ToList();
        }

        public async Task DeleteProduct(Product product)
        {
            var entity = await dbContext.Products.FirstAsync(entity => entity.Id == product.Id);
            dbContext.Products.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Product> UpdateProduct(Product newProduct)
        {
            ProductEntity entity = await dbContext.Products.FirstAsync(entity => entity.Id == newProduct.Id);
            entity.Name = newProduct.Name;
            entity.ImageId = newProduct.ImageId;

            dbContext.Products.Update(entity);
            await dbContext.SaveChangesAsync();
            return await FindProductById(newProduct.Id);
        }

        public async Task<Dictionary<Guid, Product>> FindProductsByIds(List<Guid> productIds)
        {
            var products = await this.dbContext
                .Products
                .Where(product => productIds.Contains(product.Id))
                .ToListAsync();

            return products
                .Select(entity => new Product(entity.Id, entity.Name, entity.ImageId, entity.ProductType))
                .ToDictionary(p => p.Id, p => p);
        }
}