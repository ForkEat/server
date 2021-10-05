using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class StockRepositoryTests : RepositoryTest
    {
        public StockRepositoryTests() : base(new string[]{"Stocks","Units","Products"})
        {
        }
        
        [Fact]
        public async Task InsertStock_InsertRecordInDatabase()
        {
            //Given
            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "kilogram",
                Symbol = "kg"
            };

            var product = this.dataFactory.CreateCarrotProduct();
            
            var stockToInsert = new Stock()
            {
                Quantity = 2.5,
                Unit = unit,
                Product = product
            };
            var repository = new StockRepository(context);

            // When
            await context.Products.AddAsync(product);
            await context.Units.AddAsync(unit);
            await context.SaveChangesAsync();
            var result = await repository.InsertStock(stockToInsert);

            // Then
            context.Stocks.Should().ContainSingle();

            result.Id.Should().NotBe(Guid.Empty);
            result.Quantity.Should().Be(2.5);

            var stockInDb = await context.Stocks.FirstAsync(stock => stock.Id == result.Id);
            stockInDb.Id.Should().Be(result.Id);
            stockInDb.Quantity.Should().Be(2.5);
        }
        
        [Fact]
        public async Task UpdateStock_WithExistingStock_ReturnsUpdatedStock()
        {
            //Given
            var stockId = Guid.NewGuid();

            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "kilogram",
                Symbol = "kg"
            };


            var product = this.dataFactory.CreateCarrotProduct();
            
            var stock = new Stock()
            {
                Id = stockId,
                Quantity = 2.5,
                Unit = unit,
                Product = product
            };
            
            var repository = new StockRepository(context);
            
            await context.Products.AddAsync(product);
            await context.Units.AddAsync(unit);
            await context.Stocks.AddAsync(stock);
            await context.SaveChangesAsync();
            
            //When
            stock.Quantity = 7;
            var result = await repository.UpdateStock(stock);
            
            // Then
            result.Id.Should().Be(stockId);
            result.Quantity.Should().Be(7);
            result.Unit.Should().Be(unit);
        }

        [Fact]
        public async Task DeleteStock_WithExistingStock_ReturnsVoid()
        {
            //Given
            var stockId = Guid.NewGuid();

            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "kilogram",
                Symbol = "kg"
            };


            var product = this.dataFactory.CreateCarrotProduct();

            var stock = new Stock()
            {
                Id = stockId,
                Quantity = 2.5,
                Unit = unit,
                Product = product
            };

            var repository = new StockRepository(context);

            await context.Products.AddAsync(product);
            await context.Units.AddAsync(unit);
            await context.Stocks.AddAsync(stock);
            await context.SaveChangesAsync();

            // Then
            await repository.DeleteStock(stock);
            context.Stocks.Should().BeEmpty();
        }

        [Fact]
        public async Task FindAllStocksByProductId_WithValidParams_ReturnsList()
        {
            // Given
            var productId = Guid.NewGuid();

            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "kilogram",
                Symbol = "kg"
            };


            var (product1, product2) = await this.dataFactory.CreateAndInsertProducts();

            await context.Units.AddAsync(unit);

            var stockId = Guid.NewGuid();
            var stock = this.dataFactory.CreateStock(stockId, product1, unit);
            var stock2 = this.dataFactory.CreateStock(Guid.NewGuid(), product2, unit);

            await context.Stocks.AddAsync(stock);
            await context.Stocks.AddAsync(stock2);
            await context.SaveChangesAsync();

            // When
            var repository = new StockRepository(context);
            var result = await repository.FindAllStocksByProductId(productId);

            // Then
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(stockId);
        }
    }
}