using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services;

public interface IProductTypeService
{
    Task<ProductType> CreateProductType(CreateUpdateProductTypeRequest createUpdateProductTypeRequest);
    Task<ProductType> GetProductTypeById(Guid id);
    Task<IList<ProductType>> GetAllProductTypes();
    Task DeleteProductType(Guid id);
    Task<ProductType> UpdateProductType(Guid id, CreateUpdateProductTypeRequest createUpdateProductTypeRequest);
}