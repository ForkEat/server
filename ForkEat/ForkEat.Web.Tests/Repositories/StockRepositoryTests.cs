using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Entities;
using ForkEat.Web.Database.Repositories;
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

            var (productEntity,_) = await this.dataFactory.CreateAndInsertProducts();


            var product = new Product(productEntity.Id, productEntity.Name, productEntity.ImageId);
            var stockToInsert =  new Stock(2.5, unit, product);
            var repository = new StockRepository(context);

            await context.Units.AddAsync(unit);
            await context.SaveChangesAsync();

            // When
            var result = await repository.InsertStock(stockToInsert);

            // Then
            context.Stocks.Should().ContainSingle();

            result.Id.Should().NotBe(Guid.Empty);
            result.Quantity.Should().Be(2.5);

            var stockInDb = await context
                .Stocks
                .Include(stock => stock.Product)
                .FirstAsync(stock => stock.Id == result.Id);
            
            stockInDb.Id.Should().Be(result.Id);
            stockInDb.Quantity.Should().Be(2.5);
            stockInDb.Product.Id.Should().Be(product.Id);
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
            
            var (productEntity,_) = await this.dataFactory.CreateAndInsertProducts();
            var stockEntity = new StockEntity()
            {
                Id = stockId,
                Quantity = 2.5,
                Unit = unit,
                Product = productEntity
            };
            
            await context.Units.AddAsync(unit);
            await context.Stocks.AddAsync(stockEntity);
            await context.SaveChangesAsync();
            
            var stock = new Stock(stockId, 7, unit, new Product(productEntity.Id, productEntity.Name, productEntity.ImageId));
            var repository = new StockRepository(context);

            context.Entry(stockEntity).State = EntityState.Detached;
            await context.SaveChangesAsync();
            
            //When
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


            var (product,_) = await this.dataFactory.CreateAndInsertProducts();

            var stock = new StockEntity()
            {
                Id = stockId,
                Quantity = 2.5,
                Unit = unit,
                Product = product
            };

            var repository = new StockRepository(context);

            await context.Units.AddAsync(unit);
            await context.Stocks.AddAsync(stock);
            await context.SaveChangesAsync();

            // Remove entities tracking to be able to re-query them in repo for deletion
            context.Entry(stock).State = EntityState.Detached;
            context.Entry(stock.Product).State = EntityState.Detached;
            await context.SaveChangesAsync();
            
            // Then
            await repository.DeleteStock(new Stock(stockId, 2.5, unit, new Product(product.Id, product.Name, product.ImageId)));
            context.Stocks.Should().BeEmpty();
        }

        [Fact]
        public async Task FindAllStocksByProductId_WithValidParams_ReturnsList()
        {
            // Given
            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "kilogram",
                Symbol = "kg"
            };


            var (product1, product2) = await this.dataFactory.CreateAndInsertProducts();

            await context.Units.AddAsync(unit);

            var stockId = Guid.NewGuid();
            var stock1 = new StockEntity(){Id = stockId,Quantity = 1,Unit = unit,Product = product1};
            var stock2 = new StockEntity(){Id = Guid.NewGuid(),Quantity = 1,Unit = unit,Product = product2};

            await context.Stocks.AddRangeAsync(stock1,stock2);
            await context.SaveChangesAsync();

            // When
            var repository = new StockRepository(context);
            var result = await repository.FindAllStocksByProductId(product1.Id);

            // Then
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(stockId);
        }
    }
}