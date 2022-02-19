using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly IProductTypeRepository productTypeRepository;

    public ProductService(IProductRepository productRepository, IProductTypeRepository productTypeRepository)
    {
        this.productRepository = productRepository;
        this.productTypeRepository = productTypeRepository;
    }

    public async Task<GetProductResponse> CreateProduct(CreateUpdateProductRequest createUpdateProductRequest)
    {
        var product = new Product
        (
            Guid.NewGuid(),
            createUpdateProductRequest.Name,
            createUpdateProductRequest.ImageId,
            null
        );

        product = await productRepository.InsertProduct(product);
            
        return new GetProductResponse{ Id = product.Id, Name = product.Name, ImageId = product.ImageId};
    }

    private async Task<Product> FindProductById(Guid id)
    {
        var product = await productRepository.FindProductById(id)?? throw new ProductNotFoundException();
        return product;
    }

    public async Task<GetProductResponse> GetProductById(Guid id)
    {
        var product = await FindProductById(id);
        return new GetProductResponse(product);
    }
        
    public async Task<IList<GetProductResponse>> GetAllProducts()
    {
        var products = await productRepository.FindAllProducts();
        return products
            .Select(product => new GetProductResponse() {Id = product.Id, Name = product.Name, ImageId = product.ImageId})
            .ToList();
    }

    public async Task DeleteProduct(Guid id)
    {
        var product = await FindProductById(id);
        await productRepository.DeleteProduct(product);
    }

    public async Task<GetProductResponse> UpdateProduct(Guid id, CreateUpdateProductRequest updatedProduct)
    {
        var productFromDb = await FindProductById(id);
        productFromDb.Name = updatedProduct.Name ?? productFromDb.Name;
        productFromDb.ImageId = updatedProduct.ImageId != Guid.Empty
            ? updatedProduct.ImageId
            : productFromDb.ImageId;
        productFromDb.ProductType = updatedProduct.ProductTypeId != null
            ? await productTypeRepository.FindProductTypeById(updatedProduct.ProductTypeId.Value)
            : productFromDb.ProductType;
        productFromDb =  await productRepository.UpdateProduct(productFromDb);

        return new GetProductResponse(productFromDb);
    }
}