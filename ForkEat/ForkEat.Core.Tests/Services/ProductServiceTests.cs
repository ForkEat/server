using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using Moq;
using Xunit;

namespace ForkEat.Core.Tests.Services
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateProduct_WithValidParams_ReturnsProduct()
        {
            var productName = "carrot";

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

            var productRequest = new CreateUpdateProductRequest()
            {
                Name = productName
            };

            var result = await service.CreateProduct(productRequest);
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);
        }

        [Fact]
        public async Task GetProductById_WithExistingProduct_ReturnsProduct()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();

            var mockRepository = new Mock<IProductRepository>();

            mockRepository.Setup(mock => mock.FindProductById(productId))
                .Returns<Guid>(_ => Task.FromResult(new Product
                {
                    Id = productId,
                    Name = productName
                }));

            var service = new ProductService(mockRepository.Object);

            var result = await service.GetProductById(productId);

            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be(productName);
        }

        [Fact]
        public async Task GetProductById_WithNonExistingProduct_ThrowsException()
        {
            var mockRepository = new Mock<IProductRepository>();

            mockRepository.Setup(mock => mock.FindProductById(It.IsAny<Guid>()))
                .Returns<Guid>(_ => Task.FromResult<Product>(null));

            var service = new ProductService(mockRepository.Object);

            await service.Invoking(productService => productService.GetProductById(Guid.NewGuid()))
                .Should().ThrowAsync<ProductNotFoundException>();
        }
        
        [Fact]
        public async Task GetAllProducts_ReturnsList()
        {
            var product = CreateProduct();
            var product2 = CreateProduct();

            var products = new List<Product>
            {
                product,
                product2
            };

            var mockRepository = new Mock<IProductRepository>();
            
            mockRepository.Setup(mock => mock.FindAllProducts())
                .Returns(() => Task.FromResult<List<Product>>(products));

            var service = new ProductService(mockRepository.Object);

            var result = await service.GetAllProducts();
            result.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task DeleteProduct_WithExistingProduct_ReturnsVoid()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();

            var mockRepository = new Mock<IProductRepository>();

            mockRepository.Setup(mock => mock.FindProductById(productId))
                .Returns<Guid>(_ => Task.FromResult(new Product
                {
                    Id = productId,
                    Name = productName
                }));

            var service = new ProductService(mockRepository.Object);

            await service.Invoking(productService => productService.DeleteProduct(productId))
                .Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task DeleteProduct_WithNonExistingProduct_ThrowsException()
        {
            var mockRepository = new Mock<IProductRepository>();

            mockRepository.Setup(mock => mock.FindProductById(It.IsAny<Guid>()))
                .Returns<Guid>(_ => Task.FromResult<Product>(null));

            var service = new ProductService(mockRepository.Object);

            await service.Invoking(productService => productService.DeleteProduct(Guid.NewGuid()))
                .Should().ThrowAsync<ProductNotFoundException>();
        }
        
        [Fact]
        public async Task UpdateProduct_WithExistingProduct_ReturnsProduct()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();

            var mockRepository = new Mock<IProductRepository>();

            Product updatedProduct = null;
            
            mockRepository.Setup(mock => mock.FindProductById(productId))
                .Returns<Guid>(_ => Task.FromResult(new Product
                {
                    Id = productId,
                    Name = productName
                }));

            mockRepository.Setup(mock => mock.UpdateProduct(It.IsAny<Product>()))
                .Returns<Product>(product =>
                {
                    updatedProduct = product;
                    return Task.FromResult(updatedProduct);
                });

            var service = new ProductService(mockRepository.Object);

            var updateProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot updated"
            };

            var result = await service.UpdateProduct(productId, updateProductRequest);

            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("carrot updated");
        }

        [Fact]
        public async Task UpdateProduct_WithNonExistingProduct_ThrowsException()
        {
            var productId = Guid.NewGuid();

            var updateProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot updated"
            };
            
            var mockRepository = new Mock<IProductRepository>();

            mockRepository.Setup(mock => mock.FindProductById(It.IsAny<Guid>()))
                .Returns<Guid>(_ => Task.FromResult<Product>(null));

            var service = new ProductService(mockRepository.Object);

            await service.Invoking(productService => productService.UpdateProduct(productId, updateProductRequest))
                .Should().ThrowAsync<ProductNotFoundException>();
        }
        
        private Product CreateProduct()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();
            
            return new Product
            {
                Id = productId,
                Name = productName + " " +productId
            };
        }
    }
}