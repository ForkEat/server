using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database.Entities;
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
            var product = this.dataFactory.CreateCarrotProduct();
            var repository = new ProductRepository(context);

            // When
            var result = await repository.InsertProduct(product);

            // Then
            context.Products.Should().ContainSingle();

            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("carrot");
            result.ImageId.Should().Be(product.ImageId);

            var productInDb = await context.Products.FirstAsync(p => p.Id == result.Id);
            productInDb.Id.Should().Be(result.Id);
            productInDb.Name.Should().Be("carrot");
            productInDb.ImageId.Should().Be(product.ImageId);
        }

        [Fact]
        public async Task FindProductById_ExistingProduct_ReturnsProduct()
        {
            // Given
            var productName = "carrot";
            var productId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            var product = await this.dataFactory.CreateAndInsertProduct(productId, productName, imageId);
            var repository = new ProductRepository(context);

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
            await this.dataFactory.CreateAndInsertProducts();
            var repository = new ProductRepository(context);

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

            await this.dataFactory.CreateAndInsertProducts();

            // When
            var result = await repository.FindAllProducts();

            // Then
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteProduct_WithExistingProduct_ReturnsVoid()
        {
            // Given
            var productEntity = await this.dataFactory.CreateAndInsertProduct();
            var repository = new ProductRepository(context);

            var product = new Product(productEntity.Id, productEntity.Name, productEntity.ImageId);
            
            // Then
            await repository
                .Invoking(productRepository => productRepository.DeleteProduct(product))
                .Should()
                .NotThrowAsync<Exception>();

            this.context.Products.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateProduct_WithExistingProduct_ReturnsProduct()
        {
            // Given

            var productId = Guid.NewGuid();
            var productName = "Carrot";
            var imageId = Guid.NewGuid();

            var productEntity = await this.dataFactory.CreateAndInsertProduct(productId, productName, imageId);

            var repository = new ProductRepository(context);

            var product = new Product(productId, "Carrot updated", imageId);

            //When
            
            var result = await repository.UpdateProduct(product);

            // Then
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Carrot updated");
            result.ImageId.Should().Be(imageId);

            ProductEntity productInDb = await this.context.Products
                .FirstAsync(entity => entity.Id == productId);
            
            productInDb.Id.Should().Be(productId);
            productInDb.Name.Should().Be("Carrot updated");
            productInDb.ImageId.Should().Be(imageId);

        }

        [Fact]
        public async Task FindProductsByIds_ReturnsOnlyExpectedProducts()
        {
            // Given
            var productEntities = new ProductEntity[]
            {
                new ProductEntity(){Id = Guid.NewGuid(), Name = "Potatoes",ImageId =   Guid.NewGuid() },
                new ProductEntity(){Id = Guid.NewGuid(), Name = "Carrot",  ImageId = Guid.NewGuid() },
                new ProductEntity(){Id = Guid.NewGuid(), Name = "Cabbage", ImageId = Guid.NewGuid()},
            };
            await this.context.Products.AddRangeAsync(productEntities);
            await this.context.SaveChangesAsync();

            var repository = new ProductRepository(this.context);
            
            var productIds = productEntities
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


    }
}