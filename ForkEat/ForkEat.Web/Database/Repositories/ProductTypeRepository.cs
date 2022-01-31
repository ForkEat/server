using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories;

public class ProductTypeRepository : IProductTypeRepository
{

    private readonly ApplicationDbContext dbContext;

    public ProductTypeRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ProductType> InsertProductType(ProductType productType)
    {
        await dbContext.ProductTypes.AddAsync(productType);
        await dbContext.SaveChangesAsync();
        return productType;
    }

    public Task<ProductType> FindProductTypeById(Guid id)
    {
        return dbContext
            .ProductTypes
            .FirstOrDefaultAsync(productType => productType.Id == id);
    }

    public Task<List<ProductType>> FindAllProductTypes()
    {
        return dbContext.ProductTypes.ToListAsync();
    }

    public async Task DeleteProductType(ProductType productType)
    {
        dbContext.ProductTypes.Remove(productType);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ProductType> UpdateProductType(ProductType newProductType)
    {
        dbContext.ProductTypes.Update(newProductType);
        await dbContext.SaveChangesAsync();
        return await FindProductTypeById(newProductType.Id);
    }

    public Task<Dictionary<Guid, ProductType>> FindProductTypesByIds(List<Guid> productTypeIds)
    {
        return dbContext
            .ProductTypes
            .Where(productType => productTypeIds.Contains(productType.Id))
            .ToDictionaryAsync(pt => pt.Id, pt => pt);
    }
}