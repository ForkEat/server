using System;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain
{
    public class ProductTest
    {
        [Fact]
        public void Product_IdNotEmpty()
        {
            // When
            Action a = () => new Product(Guid.Empty, "Test Name", Guid.NewGuid());
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Product_ImageIdNotEmpty()
        {
            // When
            Action a = () => new Product(Guid.NewGuid(), "Test Name", Guid.Empty);
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Product_NameNotEmpty()
        {
            // When
            Action a = () => new Product(Guid.NewGuid(), "", Guid.NewGuid());
            
            // Then
            a.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void Product_NameNotNull()
        {
            // When
            Action a = () => new Product(Guid.NewGuid(), null, Guid.NewGuid());
            
            // Then
            a.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Product_NoId_GeneratesOne()
        {
            // When
            var product = new Product("test product", Guid.NewGuid());
            
            // Then
            product.Id.Should().NotBeEmpty();
        }
    }
}