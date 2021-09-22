using System;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class UnitRepositoryTests : RepositoryTest
    {
        public UnitRepositoryTests() : base("Units")
        {
        }
        
        [Fact]
        public async Task InsertUnit_InsertRecordInDatabase()
        {
            // Given
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = unitName,
                Symbol = unitSymbol
            };
            var repository = new UnitRepository(context);
        
            // When
            var result = await repository.InsertUnit(unit);
        
            // Then
            context.Units.Should().ContainSingle();
        
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(unitName);
            result.Symbol.Should().Be(unitSymbol);
        
            var unitInDb = await context.Units.FirstAsync(unit => unit.Id == result.Id);
            unitInDb.Id.Should().Be(result.Id);
            unitInDb.Name.Should().Be(unitName);
            unitInDb.Symbol.Should().Be(unitSymbol);
        }
        
        [Fact]
        public async Task FindUnitById_ExistingUnit_ReturnsUnit()
        {
            // Given
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();
            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };
            var repository = new UnitRepository(context);
            
            await context.Units.AddAsync(unit);
            await context.SaveChangesAsync();
        
            // When
            var result = await repository.FindUnitById(unitId);
            
            // Then
            result.Id.Should().Be(result.Id);
            result.Name.Should().Be(unitName);
            result.Symbol.Should().Be(unitSymbol);
        }
        
        [Fact]
        public async Task FindUnitById_NonExistingUnit_ReturnsNull()
        {
            // Given
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();
            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };
            var repository = new UnitRepository(context);
            
            await context.Units.AddAsync(unit);
            await context.SaveChangesAsync();
        
            // When
            var result = await repository.FindUnitById(Guid.NewGuid());
            
            // Then
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task FindAllUnits_ReturnsList()
        {
            // Given
            var repository = new UnitRepository(context);
        
            var unit = CreateUnit("kilograms", "kg");
            var unit2 = CreateUnit("units", "ut");
        
            await context.Units.AddAsync(unit);
            await context.Units.AddAsync(unit2);
            await context.SaveChangesAsync();
        
            // When
            var result = await repository.FindAllUnits();
            
            // Then
            result.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task DeleteUnit_WithExistingUnit_ReturnsVoid()
        {
            // Given
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();
            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };
            var repository = new UnitRepository(context);
            
            await context.Units.AddAsync(unit);
            await context.SaveChangesAsync();
        
            // Then
            await repository.Invoking(unitRepository => unitRepository.DeleteUnit(unit))
                .Should().NotThrowAsync<Exception>();
        }
        
        [Fact]
        public async Task UpdateUnit_WithExistingUnit_ReturnsUnit()
        {
            // Given
            var unitName = "kilogram";
            var unitSymbol = "kg";
            var unitId = Guid.NewGuid();
            var unit = new Unit()
            {
                Id = unitId,
                Name = unitName,
                Symbol = unitSymbol
            };
        
            var repository = new UnitRepository(context);
            
            await context.Units.AddAsync(unit);
            await context.SaveChangesAsync();
            
            //When
            unit.Name = "kilogram updated";
            var result = await repository.UpdateUnit(unit);
        
            // Then
            result.Id.Should().Be(unitId);
            result.Name.Should().Be(unitName + " updated");
            result.Symbol.Should().Be(unitSymbol);
        }
        
        private Unit CreateUnit(string name, string symbol)
        {
            var unitId = Guid.NewGuid();
            
            return new Unit()
            {
                Id = unitId,
                Name = name,
                Symbol = symbol
            };
        }
    }
}