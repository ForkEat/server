using System;
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
        private ApplicationDbContext context;

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
    }
}