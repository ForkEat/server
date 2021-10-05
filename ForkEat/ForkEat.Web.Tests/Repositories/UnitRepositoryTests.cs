using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class UnitRepositoryTests : RepositoryTest
    {
        public UnitRepositoryTests() : base(new string[]{"Units"})
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
        
            var unit = this.dataFactory.CreateUnit("kilograms", "kg");
            var unit2 = this.dataFactory.CreateUnit("units", "ut");
        
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
        
        [Fact]
        public async Task FindProductsByIds_ReturnsOnlyExpectedProducts()
        {
            // Given
            var units = new Unit[]
            {
                new Unit() { Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg" },
                new Unit() { Id = Guid.NewGuid(), Name = "Litre", Symbol = "L" },
                new Unit() { Id = Guid.NewGuid(), Name = "Gramme", Symbol ="g"},
            };
            await this.context.Units.AddRangeAsync(units);
            await this.context.SaveChangesAsync();

            var repository = new UnitRepository(this.context);
            
            var unitsIds = units
                .Take(2)
                .Select(product => product.Id)
                .ToList();

            // When
            var result = await repository.FindUnitsByIds(unitsIds);

            // Then
            result.Should().HaveCount(2);
            result.Should().ContainKeys(unitsIds);
            result[unitsIds[0]].Name.Should().Be("Kilogramme");
            result[unitsIds[1]].Name.Should().Be("Litre");
        }


    }
}