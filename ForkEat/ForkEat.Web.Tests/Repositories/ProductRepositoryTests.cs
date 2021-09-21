using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class ProductRepositoryTests : RepositoryTest
    {

        public ProductRepositoryTests() : base("Products")
        {
        }

        [Fact]
        public async Task InsertProduct_InsertRecordInDatabase()
        {
            // Given
            var productName = "carrott";
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = productName
            };
            var repository = new ProductRepository(context);

            // When
            var result = await repository.InsertProduct(product);

            // Then
            context.Products.Should().ContainSingle();

            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);

            var productInDb = await context.Products.FirstAsync(product => product.Id == result.Id);
            productInDb.Id.Should().Be(result.Id);
            productInDb.Name.Should().Be(productName);
        }
        
        [Fact]
        public async Task FindProductById_ExistingProduct_ReturnsProduct()
        {
            // Given
            var productName = "carrott";
            var productId = Guid.NewGuid();
            
            var product = new Product()
            {
                Id = productId,
                Name = productName
            };
            var repository = new ProductRepository(context);
            
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // When
            var result = await repository.FindProductById(productId);
            
            // Then
            result.Id.Should().Be(result.Id);
            result.Name.Should().Be(productName);
        }
        
        [Fact]
        public async Task FindProductById_NonExistingProduct_ReturnsNull()
        {
            // Given
            var productName = "carrott";
            var productId = Guid.NewGuid();
            
            var product = new Product()
            {
                Id = productId,
                Name = productName
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

        private Product CreateProduct()
        {
            var productName = "carrott";
            var productId = Guid.NewGuid();
            
            return new Product
            {
                Id = productId,
                Name = productName + " " +productId
            };
        }
    }
}