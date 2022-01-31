using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories;

public interface IProductTypeRepository
{
    Task<ProductType> InsertProductType(ProductType productType);
    Task<ProductType> FindProductTypeById(Guid id);
    Task<List<ProductType>> FindAllProductTypes();
    Task DeleteProductType(ProductType productType);
    Task<ProductType> UpdateProductType(ProductType newProductType);
    Task<Dictionary<Guid,ProductType>> FindProductTypesByIds(List<Guid> productTypeIds);
}