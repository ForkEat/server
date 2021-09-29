using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class ProductRepositoryTests : RepositoryTest
    {
        public ProductRepositoryTests() : base(new string[] { "Products" })
        {
        }

        [Fact]
        public async Task InsertProduct_InsertRecordInDatabase()
        {
            // Given
            var productName = "carrot";
            var imageId = Guid.NewGuid();
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = productName,
                ImageId = imageId
            };
            var repository = new ProductRepository(context);

            // When
            var result = await repository.InsertProduct(product);

            // Then
            context.Products.Should().ContainSingle();

            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);
            result.ImageId.Should().Be(imageId);

            var productInDb = await context.Products.FirstAsync(product => product.Id == result.Id);
            productInDb.Id.Should().Be(result.Id);
            productInDb.Name.Should().Be(productName);
            productInDb.ImageId.Should().Be(imageId);
        }

        [Fact]
        public async Task FindProductById_ExistingProduct_ReturnsProduct()
        {
            // Given
            var productName = "carrot";
            var productId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var product = new Product()
            {
                Id = productId,
                Name = productName,
                ImageId = imageId
            };
            var repository = new ProductRepository(context);

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // When
            var result = await repository.FindProductById(productId);

            // Then
            result.Id.Should().Be(result.Id);
            result.Name.Should().Be(productName);
            result.ImageId.Should().Be(imageId);
        }

        [Fact]
        public async Task FindProductById_NonExistingProduct_ReturnsNull()
        {
            // Given
            var productName = "carrot";
            var productId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var product = new Product()
            {
                Id = productId,
                Name = productName,
                ImageId = imageId
            };
            var repository = new ProductRepository(context);

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // When
            var result = await repository.FindProductById(Guid.NewGuid());

            // Then
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindAllProducts_ReturnsList()
        {
            // Given
            var repository = new ProductRepository(context);

            var product = CreateProduct();
            var product2 = CreateProduct();

            await context.Products.AddAsync(product);
            await context.Products.AddAsync(product2);
            await context.SaveChangesAsync();

            // When
            var result = await repository.FindAllProducts();

            // Then
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteProduct_WithExistingProduct_ReturnsVoid()
        {
            // Given
            var productName = "carrot";
            var productId = Guid.NewGuid();

            var product = new Product()
            {
                Id = productId,
                Name = productName,
                ImageId = Guid.NewGuid()
            };
            var repository = new ProductRepository(context);

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Then
            await repository.Invoking(productRepository => productRepository.DeleteProduct(product))
                .Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateProduct_WithExistingProduct_ReturnsProduct()
        {
            // Given
            var productName = "carrot";
            var productId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var product = new Product()
            {
                Id = productId,
                Name = productName,
                ImageId = imageId
            };

            var repository = new ProductRepository(context);

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            //When
            product.Name = "carrot updated";
            var result = await repository.UpdateProduct(product);

            // Then
            result.Id.Should().Be(productId);
            result.Name.Should().Be(productName + " updated");
            result.ImageId.Should().Be(imageId);
        }

        [Fact]
        public async Task FindProductsByIds_ReturnsOnlyExpectedProducts()
        {
            // Given
            var products = new Product[]
            {
                new Product() { Id = Guid.NewGuid(), Name = "Potatoes", ImageId = Guid.NewGuid() },
                new Product() { Id = Guid.NewGuid(), Name = "Carrot", ImageId = Guid.NewGuid() },
                new Product() { Id = Guid.NewGuid(), Name = "Cabbage", ImageId = Guid.NewGuid()},
            };
            await this.context.Products.AddRangeAsync(products);
            await this.context.SaveChangesAsync();

            var repository = new ProductRepository(this.context);
            
            var productIds = products
                .Take(2)
                .Select(product => product.Id)
                .ToList();

            // When
            var result = await repository.FindProductsByIds(productIds);

            // Then
            result.Should().HaveCount(2);
            result.Should().ContainKeys(productIds);
            result[productIds[0]].Name.Should().Be("Potatoes");
            result[productIds[1]].Name.Should().Be("Carrot");
        }

        private Product CreateProduct()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();

            return new Product
            {
                Id = productId,
                Name = productName + " " + productId,
                ImageId = Guid.NewGuid()
            };
        }
    }
}