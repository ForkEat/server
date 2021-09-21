using System;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
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

        public async Task<Product> CreateProduct(CreateProductRequest createProductRequest)
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = createProductRequest.Name
            };

            return await productRepository.InsertProduct(product);
        }
    }
}