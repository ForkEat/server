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

namespace ForkEat.Core.Tests.Services;

public class UnitServiceTests
{
    [Fact]
    public async Task CreateUnit_WithValidParams_ReturnsUnit()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";

        var mockRepository = new Mock<IUnitRepository>();

        Unit insertedUnit = null;

        mockRepository.Setup(mock => mock.InsertUnit(It.IsAny<Unit>()))
            .Returns<Unit>(unit =>
            {
                insertedUnit = unit;
                insertedUnit.Id = Guid.NewGuid();
                return Task.FromResult(insertedUnit);
            });

        var service = new UnitService(mockRepository.Object);

        var unitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };

        var result = await service.CreateUnit(unitRequest);
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(unitName);
        result.Symbol.Should().Be(unitSymbol);
    }

    [Fact]
    public async Task GetUnitById_WithExistingUnit_ReturnsUnit()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var unitId = Guid.NewGuid();
        
        var mockRepository = new Mock<IUnitRepository>();
        
        mockRepository.Setup(mock => mock.FindUnitById(unitId))
            .Returns<Guid>(_ => Task.FromResult(new Unit
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            }));
        
        var service = new UnitService(mockRepository.Object);
        
        var result = await service.GetUnitById(unitId);
        
        result.Should().NotBeNull();
        result.Id.Should().Be(unitId);
        result.Name.Should().Be(unitName);
        result.Symbol.Should().Be(unitSymbol);
    }
        
    [Fact]
    public async Task GetUnitById_WithNonExistingUnit_ThrowsException()
    {
        var mockRepository = new Mock<IUnitRepository>();
        
        mockRepository.Setup(mock => mock.FindUnitById(It.IsAny<Guid>()))
            .Returns<Guid>(_ => Task.FromResult<Unit>(null));
        
        var service = new UnitService(mockRepository.Object);
        
        await service.Invoking(unitService => unitService.GetUnitById(Guid.NewGuid()))
            .Should().ThrowAsync<UnitNotFoundException>();
    }
        
    [Fact]
    public async Task GetAllUnits_ReturnsList()
    {
        var unit = CreateUnit("kilogram", "kg");
        var unit2 = CreateUnit("units", "ut");
        
        var units = new List<Unit>
        {
            unit,
            unit2
        };
        
        var mockRepository = new Mock<IUnitRepository>();
            
        mockRepository.Setup(mock => mock.FindAllUnits())
            .Returns(() => Task.FromResult<List<Unit>>(units));
        
        var service = new UnitService(mockRepository.Object);
        
        var result = await service.GetAllUnits();
        result.Should().HaveCount(2);
    }
        
    [Fact]
    public async Task DeleteUnit_WithExistingUnit_ReturnsVoid()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var unitId = Guid.NewGuid();
        
        var mockRepository = new Mock<IUnitRepository>();
        
        mockRepository.Setup(mock => mock.FindUnitById(unitId))
            .Returns<Guid>(_ => Task.FromResult(new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            }));
        
        var service = new UnitService(mockRepository.Object);
        
        await service.Invoking(unitService => unitService.DeleteUnit(unitId))
            .Should().NotThrowAsync<Exception>();
    }
        
    [Fact]
    public async Task DeleteUnit_WithNonExistingUnit_ThrowsException()
    {
        var mockRepository = new Mock<IUnitRepository>();
        
        mockRepository.Setup(mock => mock.FindUnitById(It.IsAny<Guid>()))
            .Returns<Guid>(_ => Task.FromResult<Unit>(null));
        
        var service = new UnitService(mockRepository.Object);
        
        await service.Invoking(unitService => unitService.DeleteUnit(Guid.NewGuid()))
            .Should().ThrowAsync<UnitNotFoundException>();
    }
        
    [Fact]
    public async Task UpdateUnit_WithExistingUnit_ReturnsUnit()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var unitId = Guid.NewGuid();
        
        var mockRepository = new Mock<IUnitRepository>();
        
        Unit updatedUnit = null;
            
        mockRepository.Setup(mock => mock.FindUnitById(unitId))
            .Returns<Guid>(_ => Task.FromResult(new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            }));
        
        mockRepository.Setup(mock => mock.UpdateUnit(It.IsAny<Unit>()))
            .Returns<Unit>(unit =>
            {
                updatedUnit = unit;
                return Task.FromResult(updatedUnit);
            });
        
        var service = new UnitService(mockRepository.Object);
        
        var updateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = "kilogram updated"
        };
        
        var result = await service.UpdateUnit(unitId, updateUnitRequest);
        
        result.Should().NotBeNull();
        result.Id.Should().Be(unitId);
        result.Name.Should().Be("kilogram updated");
        result.Symbol.Should().Be(unitSymbol);
    }
        
    [Fact]
    public async Task UpdateUnit_WithNonExistingUnit_ThrowsException()
    {
        var unitId = Guid.NewGuid();
        
        var updateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = "kilogram updated"
        };
            
        var mockRepository = new Mock<IUnitRepository>();
        
        mockRepository.Setup(mock => mock.FindUnitById(It.IsAny<Guid>()))
            .Returns<Guid>(_ => Task.FromResult<Unit>(null));
        
        var service = new UnitService(mockRepository.Object);
        
        await service.Invoking(unitService => unitService.UpdateUnit(unitId, updateUnitRequest))
            .Should().ThrowAsync<UnitNotFoundException>();
    }
        
    private Unit CreateUnit(string unitName, string unitSymbol)
    {
        var unitId = Guid.NewGuid();
            
        return new Unit()
        {
            Id = unitId,
            Name = unitName,
            Symbol = unitSymbol
        };
    }
}