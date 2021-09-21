using System;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using Moq;
using Xunit;

namespace ForkEat.Core.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateProduct_WithValidParams_ReturnsProduct()
        {
            var productName = "carrott";
            
            var mockRepository = new Mock<IProductRepository>();

            Product insertedProduct = null;

            mockRepository.Setup(mock => mock.InsertProduct(It.IsAny<Product>()))
                .Returns<Product>(product =>
                {
                    insertedProduct = product;
                    insertedProduct.Id = Guid.NewGuid();
                    return Task.FromResult(insertedProduct);
                });
            
            var service = new ProductService(mockRepository.Object);

            var productRequest = new CreateProductRequest()
            {
                Name = productName
            };

            var result = await service.CreateProduct(productRequest);
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);
        }
    }
}