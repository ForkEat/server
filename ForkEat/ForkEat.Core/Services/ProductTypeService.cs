using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services;

public class ProductTypeService : IProductTypeService
{
    private readonly IProductTypeRepository productTypeRepository;

    public ProductTypeService(IProductTypeRepository productTypeRepository)
    {
        this.productTypeRepository = productTypeRepository;
    }

    public async Task<ProductType> CreateProductType(CreateUpdateProductTypeRequest createUpdateProductTypeRequest)
    {
        var productType = new ProductType
        {
            Id = Guid.NewGuid(),
            Name = createUpdateProductTypeRequest.Name,
        };

        return await productTypeRepository.InsertProductType(productType);
    }

    public async Task<ProductType> GetProductTypeById(Guid id)
    {
        var productType = await productTypeRepository.FindProductTypeById(id);

        return productType ?? throw new ProductTypeNotFoundException();
    }

    public async Task<IList<ProductType>> GetAllProductTypes()
    {
        return await productTypeRepository.FindAllProductTypes();
    }

    public async Task DeleteProductType(Guid id)
    {
        var productType = await GetProductTypeById(id);
        await productTypeRepository.DeleteProductType(productType);
    }

    public async Task<ProductType> UpdateProductType(Guid id, CreateUpdateProductTypeRequest createUpdateProductTypeRequest)
    {
        var productTypeFromDb = await GetProductTypeById(id);
        productTypeFromDb.Name = createUpdateProductTypeRequest.Name ?? productTypeFromDb.Name;
        return await productTypeRepository.UpdateProductType(productTypeFromDb);
    }
}