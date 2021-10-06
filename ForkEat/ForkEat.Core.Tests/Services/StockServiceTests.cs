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
    public class StockServiceTests
    {
        [Fact]
        public async Task CreateOrUpdateStock_WithNonExistingProduct_ThrowsException()
        {
            var productId = Guid.NewGuid();

            var stock = new CreateUpdateStockRequest()
            {
                Quantity = 2
            };

            var stockMockRepository = new Mock<IStockRepository>();
            var productMockRepository = new Mock<IProductRepository>();
            var unitMockRepository = new Mock<IUnitRepository>();

            productMockRepository.Setup(mock => mock.FindProductById(It.IsAny<Guid>()))
                .Returns<Guid>(_ => Task.FromResult<Product>(null));

            var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);

            await service.Invoking(stockService => stockService.CreateOrUpdateStock(productId, stock))
                .Should().ThrowAsync<ProductNotFoundException>();
        }

        [Fact]
        public async Task CreateOrUpdateStock_WithNonExistingStock_CreatesStock()
        {
            var stockQuantity = 7;
            var productId = Guid.NewGuid();
            var productName = "carrot";
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();

            var stockMockRepository = new Mock<IStockRepository>();
            var productMockRepository = new Mock<IProductRepository>();
            var unitMockRepository = new Mock<IUnitRepository>();

            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };

            Stock insertedStock = null;

            productMockRepository.Setup(mock => mock.FindProductById(productId))
                .Returns<Guid>(_ => Task.FromResult(new Product(productId,productName, Guid.NewGuid())));

            unitMockRepository.Setup(mock => mock.FindUnitById(unitId))
                .Returns<Guid>(_ => Task.FromResult(new Unit()
                {
                    Id = unitId,
                    Name = unitName,
                    Symbol = unitSymbol
                }));

            stockMockRepository.Setup(mock => mock.InsertStock(It.IsAny<Stock>()))
                .Returns<Stock>(stock =>
                {
                    insertedStock = stock;
                    return Task.FromResult(insertedStock);
                });

            var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);

            var stock = new CreateUpdateStockRequest()
            {
                Quantity = stockQuantity,
                UnitId = unit.Id
            };
            var result = await service.CreateOrUpdateStock(productId, stock);
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.Quantity.Should().Be(stockQuantity);
            result.Unit.Name.Should().Be(unitName);
            result.Unit.Symbol.Should().Be(unitSymbol);
        }

        [Fact]
        public async Task CreateOrUpdateStock_WithExistingStock_UpdatesStock()
        {
            // Given
            var stockId = Guid.NewGuid();
            var stockQuantity = 7;
            var stockUpdatedQuantity = 2;
            var productId = Guid.NewGuid();
            var productName = "carrot";
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();

            var product = new Product("Test Product", Guid.NewGuid());
            
            var stockMockRepository = new Mock<IStockRepository>();
            var productMockRepository = new Mock<IProductRepository>();
            var unitMockRepository = new Mock<IUnitRepository>();

            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };

         
            productMockRepository.Setup(mock => mock.FindProductById(productId))
                .Returns<Guid>(_ => Task.FromResult(new Product(productId,productName, Guid.NewGuid())));

            unitMockRepository.Setup(mock => mock.FindUnitById(unitId))
                .Returns<Guid>(_ => Task.FromResult(new Unit()
                {
                    Id = unitId,
                    Name = unitName,
                    Symbol = unitSymbol
                }));

            stockMockRepository
                .Setup(mock => mock.FindStockById(stockId))
                .Returns<Guid>(_ => Task.FromResult(new Stock(stockId,stockQuantity,unit, product)));

            stockMockRepository
                .Setup(mock => mock.UpdateStock(It.IsAny<Stock>()))
                .Returns<Stock>(_ => Task.FromResult(new Stock(stockId,stockUpdatedQuantity,unit, product)));

            var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);

            var stock = new CreateUpdateStockRequest()
            {
                Id = stockId,
                Quantity = stockUpdatedQuantity,
                UnitId = unitId
            };
            
            // When
            var result = await service.CreateOrUpdateStock(productId, stock);
            
            // Then
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.Quantity.Should().Be(stockUpdatedQuantity);
            result.Unit.Name.Should().Be(unitName);
            result.Unit.Symbol.Should().Be(unitSymbol);
        }

        [Fact]
        public async Task CreateOrUpdateStock_With0Quantity_DeletesStock()
        {
            var stockId = Guid.NewGuid();
            var stockQuantity = 7;
            var stockUpdatedQuantity = 0;
 
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();

            var product = new Product("Test Product", Guid.NewGuid());
            
            var stockMockRepository = new Mock<IStockRepository>();
            var productMockRepository = new Mock<IProductRepository>();
            var unitMockRepository = new Mock<IUnitRepository>();

            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };

            productMockRepository.Setup(mock => mock.FindProductById(product.Id))
                .Returns<Guid>(_ => Task.FromResult(product));

            unitMockRepository.Setup(mock => mock.FindUnitById(unitId))
                .Returns<Guid>(_ => Task.FromResult(new Unit()
                {
                    Id = unitId,
                    Name = unitName,
                    Symbol = unitSymbol
                }));

            stockMockRepository.Setup(mock => mock.FindStockById(stockId))
                .Returns<Guid>(_ => Task.FromResult(new Stock(stockId,stockQuantity,unit, product)));

            var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);

            var stock = new CreateUpdateStockRequest()
            {
                Id = stockId,
                Quantity = stockUpdatedQuantity,
                UnitId = unitId
            };

            var stocks = await service.GetStocks(product.Id);
            stocks.Should().BeEmpty();
        }

        [Fact]
        public async Task GetStocks_ReturnsList()
        {
            var product = new Product("Test Product", Guid.NewGuid());

            var stock = new Stock(Guid.NewGuid(), 3, new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "kilogram",
                Symbol = "kg"
            },product);
            
            var stocks = new List<Stock>
            {
                stock
            };

            var stockMockRepository = new Mock<IStockRepository>();
            var productMockRepository = new Mock<IProductRepository>();
            var unitMockRepository = new Mock<IUnitRepository>();

            stockMockRepository
                .Setup(stockRepository => stockRepository.FindAllStocksByProductId(product.Id))
                .Returns<Guid>(_ => Task.FromResult<IEnumerable<Stock>>(stocks));

            var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);

            var result = await service.GetStocks(product.Id);
            result.Should().HaveCount(1);
        }
    }
}