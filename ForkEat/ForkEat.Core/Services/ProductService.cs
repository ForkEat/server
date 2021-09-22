using System;
using System.Collections.Generic;
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

        public async Task<Product> CreateProduct(CreateUpdateProductRequest createUpdateProductRequest)
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = createUpdateProductRequest.Name
            };

            return await productRepository.InsertProduct(product);
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = await productRepository.FindProductById(id);
            return product ?? throw new ProductNotFoundException();
        }

        public async Task<IList<Product>> GetAllProducts()
        {
            return await productRepository.FindAllProducts();
        }

        public async Task DeleteProduct(Guid id)
        {
            var product = await GetProductById(id);
            await productRepository.DeleteProduct(product);
        }

        public async Task<Product> UpdateProduct(Guid id, CreateUpdateProductRequest updatedProduct)
        {
            var productFromDb = await GetProductById(id);
            productFromDb.Name = updatedProduct.Name;
            return await productRepository.UpdateProduct(productFromDb);
        }
    }
}