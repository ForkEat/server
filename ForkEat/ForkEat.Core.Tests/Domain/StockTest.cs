using System;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain
{
    public class StockTest
    {
        private readonly Unit unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};
        private readonly Product product = new Product(Guid.NewGuid(), "Test Product", Guid.NewGuid());

        
        [Fact]
        public void Stock_IdNotEmpty()
        {
            // When
            Action a = () => new Stock(Guid.Empty, 1,unit, product);
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Stock_CreatedWithNoId_GeneratesOne()
        {
            // When
            var stock = new Stock(1,unit, product);
            
            // Then
            stock.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Stock_QuantityNot0()
        {
            // When
            Action a = () => new Stock(0,unit, product);
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Stock_QuantityNotBelow0()
        {
            // When
            Action a = () => new Stock(-23,unit, product);
            
            // Then
            a.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void Stock_ProductNotNull()
        {
            // When
            Action a = () => new Stock(1,unit, null);
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Stock_UnitNotNull()
        {
            // When
            Action a = () => new Stock(1,null, product);
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
    }
}