using System;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain
{
    public class IngredientTest
    {
        [Fact]
        public void Quantity_Not0()
        {
            // Given
            var ingredient = new Ingredient(new Product(){Id = Guid.NewGuid(), Name = "Test product"}, 1);
            
            // When  & Then
            ingredient.Invoking(i => i.Quantity = 0)
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Ingredient Quantity should be positive");
        }
        
        [Fact]
        public void Product_NotNull()
        {
            // Given
            var ingredient = new Ingredient(new Product(){Id = Guid.NewGuid(), Name = "Test product"}, 1);
            
            // When  & Then
            ingredient.Invoking(i => i.Product = null)
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Ingredient Product should not be null");
        }
    }
}