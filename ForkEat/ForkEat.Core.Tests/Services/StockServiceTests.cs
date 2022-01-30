using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using Moq;
using Xunit;

namespace ForkEat.Core.Tests.Services;

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
            .Setup(mock => mock.FindStockByProductId(productId))
            .Returns<Guid>(_ => Task.FromResult(new Stock(stockId,stockQuantity,unit, product)));

        stockMockRepository
            .Setup(mock => mock.UpdateStock(It.IsAny<Stock>()))
            .Returns<Stock>(_ => Task.FromResult(new Stock(stockId,stockUpdatedQuantity,unit, product)));

        var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);

        var stock = new CreateUpdateStockRequest()
        {
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
        // Given
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

        productMockRepository
            .Setup(mock => mock.FindProductById(product.Id))
            .Returns<Guid>(_ => Task.FromResult(product));

        unitMockRepository
            .Setup(mock => mock.FindUnitById(unitId))
            .Returns<Guid>(_ => Task.FromResult(new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            }));

        stockMockRepository
            .Setup(mock => mock.FindStockByProductId(product.Id))
            .Returns<Guid>(_ => Task.FromResult(new Stock(stockId,stockQuantity,unit, product)));

        var service = new StockService(stockMockRepository.Object, productMockRepository.Object, unitMockRepository.Object);
            
        var stock = new CreateUpdateStockRequest()
        {
            Quantity = stockUpdatedQuantity,
            UnitId = unitId
        };

        // When
        await service.CreateOrUpdateStock(product.Id, stock);
            
        // Then
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

    [Fact]
    public async Task GetCompleteStock()
    {
        // Given
        var product1 = new Product("Test Product 1", Guid.NewGuid());
        var product2 = new Product("Test Product 2", Guid.NewGuid());
            
        var unit = new Unit()
        {
            Id = Guid.NewGuid(),
            Name = "Kilogramme",
            Symbol = "kg"
        };

        var stock1 = new Stock(4, unit, product1){ PurchaseDate = DateOnly.FromDateTime(DateTime.Today), BestBeforeDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2))};
        var stock2 = new Stock(2, unit, product2){ PurchaseDate = DateOnly.FromDateTime(DateTime.Today), BestBeforeDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4))};

        var stockRepoMock = new Mock<IStockRepository>();
        stockRepoMock.Setup(mock => mock.FindAllStocks())
            .Returns(() => Task.FromResult(new List<Stock> { stock1, stock2 }));

        IStockService service = new StockService(stockRepoMock.Object, null, null);

        // When
        List<ProductStockResponse> result = await service.GetCompleteStock();
            
        // Then
        result.Should().HaveCount(2);
            
        ProductStockResponse stockProduct1 = result.First(stock => stock.Product.Id == product1.Id);
        ProductStockResponse stockProduct2 = result.First(stock => stock.Product.Id == product2.Id);

        stockProduct1.Product.Name.Should().Be(product1.Name);
        stockProduct1.Product.ImageId.Should().Be(product1.ImageId);
        stockProduct1.Quantity.Should().Be(4);
        stockProduct1.Unit.Id.Should().Be(unit.Id);
        stockProduct1.Unit.Name.Should().Be(unit.Name);
        stockProduct1.Unit.Symbol.Should().Be(unit.Symbol);
        stockProduct1.BestBeforeDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(2)));
        stockProduct1.PurchaseDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.Date));

        stockProduct2.Product.Name.Should().Be(product2.Name);
        stockProduct2.Product.ImageId.Should().Be(product2.ImageId);
        stockProduct2.Quantity.Should().Be(2);
        stockProduct2.Unit.Id.Should().Be(unit.Id);
        stockProduct2.Unit.Name.Should().Be(unit.Name);
        stockProduct2.Unit.Symbol.Should().Be(unit.Symbol);
        stockProduct2.BestBeforeDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(4)));
        stockProduct2.PurchaseDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));
    }
}