using System;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain;

public class IngredientTest
{
    [Fact]
    public void Quantity_Not0()
    {
        // Given
        var ingredient = new Ingredient(
            1, new Product(Guid.NewGuid(),"Test product", Guid.NewGuid(), null),
            new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "Kilogramme", Symbol = "kg"
            });
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
        var ingredient = new Ingredient(
            1,new Product(Guid.NewGuid(),"Test product", Guid.NewGuid(), null),
            new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "Kilogramme", Symbol = "kg"
            });
        // When  & Then
        ingredient.Invoking(i => i.Product = null)
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Ingredient Product should not be null");
    }

    [Fact]
    public void Unit_NotNull()
    {
        // Given
        var ingredient = new Ingredient(
            1, new Product(Guid.NewGuid(),"Test product", Guid.NewGuid(), null),
            new Unit()
            {
                Id = Guid.NewGuid(),
                Name = "Kilogramme", Symbol = "kg"
            });

        // When  & Then
        ingredient.Invoking(i => i.Unit = null)
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Ingredient Unit should not be null");
    }
}