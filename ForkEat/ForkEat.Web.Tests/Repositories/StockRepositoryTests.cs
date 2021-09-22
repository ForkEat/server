using System;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class StockRepositoryTests : RepositoryTest
    {
        public StockRepositoryTests() : base("Stocks")
        {
        }
        
        // [Fact]
        // public async Task UpdateStock_WithExistingStock_ReturnsStock()
        // {
            // Given
            // var stockId = Guid.NewGuid();
            //
            // var unit
            //
            // var product = new Stock()
            // {
            //     Id = stockId,
            //     Quantity = 2.5,
            //     Unit = 
            // };
            //
            // var repository = new ProductRepository(context);
            //
            // await context.Products.AddAsync(product);
            // await context.SaveChangesAsync();
            //
            // //When
            // product.Name = "carrot updated";
            // var result = await repository.UpdateProduct(product);
            //
            // // Then
            // result.Id.Should().Be(productId);
            // result.Name.Should().Be(productName + " updated");
        // }
    }
}