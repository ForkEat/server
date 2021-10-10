using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<GetProductResponse> CreateProduct(CreateUpdateProductRequest createUpdateProductRequest)
        {
            var product = new Product
            (
                Guid.NewGuid(),
                createUpdateProductRequest.Name,
                createUpdateProductRequest.ImageId
            );

            product = await productRepository.InsertProduct(product);
            
            return new GetProductResponse(){ Id = product.Id, Name = product.Name, ImageId = product.ImageId};
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = await productRepository.FindProductById(id)?? throw new ProductNotFoundException();
            return product;
        }
        
        private async 

        public async Task<IList<GetProductResponse>> GetAllProducts()
        {
            var products = await productRepository.FindAllProducts();
            return products
                .Select(product => new GetProductResponse() {Id = product.Id, Name = product.Name, ImageId = product.ImageId})
                .ToList();
        }

        public async Task DeleteProduct(Guid id)
        {
            var product = await GetProductById(id);
            await productRepository.DeleteProduct(product);
        }

        public async Task<GetProductResponse> UpdateProduct(Guid id, CreateUpdateProductRequest updatedProduct)
        {
            var productFromDb = await GetProductById(id);
            productFromDb.Name = updatedProduct.Name ?? productFromDb.Name;
            productFromDb.ImageId = updatedProduct.ImageId != Guid.Empty
                ? updatedProduct.ImageId
                : productFromDb.ImageId;
            return await productRepository.UpdateProduct(productFromDb);
        }
    }
}