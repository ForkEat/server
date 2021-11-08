using System;
using System.Collections.Generic;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain
{
    public class KitchenTest
    {
        [Fact]
        public void CookRecipe_RemovesIngredientsFromStock()
        {
            // Given
            var milk = new Product("Milk", Guid.NewGuid());
            var floor = new Product("Floor", Guid.NewGuid());
            var egg = new Product("Eggs", Guid.NewGuid());
            var tomatoes = new Product("Tomatoes", Guid.NewGuid());

            var g = new Unit() {Id = Guid.NewGuid(), Name = "Gramme", Symbol = "g"};
            var L = new Unit() {Id = Guid.NewGuid(), Name = "Litre", Symbol = "L"};
            var n = new Unit() {Id = Guid.NewGuid(), Name = "Number", Symbol = ""};

            var milkStock = new Stock(1, L, milk);
            var floorStock = new Stock(1000, g, floor);
            var eggStock = new Stock(6, n, egg);
            var tomatoStock = new Stock(4, n, tomatoes);

            var recipe = new Recipe(
                Guid.NewGuid(),
                "Pancakes",
                1,
                new List<Step>(),
                new List<Ingredient>()
                {
                    new Ingredient(0.5, milk, L),
                    new Ingredient(250, floor, g),
                    new Ingredient(3, egg, n)
                },
                Guid.NewGuid()
            );

            IKitchen kitchen = new Kitchen();

            // When
            kitchen.CookRecipeFromStock(recipe, new List<Stock> {milkStock,tomatoStock, floorStock, eggStock});

            // Then
            milkStock.Quantity.Should().Be(0.5);
            floorStock.Quantity.Should().Be(750);
            eggStock.Quantity.Should().Be(3);
            tomatoStock.Quantity.Should().Be(4);
        }

        [Fact]
        public void CookRecipe_DifferentUnit_ThrowsException()
        {
            // Given
            var milk = new Product("Milk", Guid.NewGuid());
            var floor = new Product("Floor", Guid.NewGuid());
            var egg = new Product("Eggs", Guid.NewGuid());
            var tomatoes = new Product("Tomatoes", Guid.NewGuid());

            var g = new Unit() {Id = Guid.NewGuid(), Name = "Gramme", Symbol = "g"};
            var L = new Unit() {Id = Guid.NewGuid(), Name = "Litre", Symbol = "L"};
            var n = new Unit() {Id = Guid.NewGuid(), Name = "Number", Symbol = ""};

            var milkStock = new Stock(1, g, milk);
            var floorStock = new Stock(1000, g, floor);
            var eggStock = new Stock(6, n, egg);
            var tomatoStock = new Stock(4, n, tomatoes);

            var recipe = new Recipe(
                Guid.NewGuid(),
                "Pancakes",
                1,
                new List<Step>(),
                new List<Ingredient>()
                {
                    new Ingredient(0.5, milk, L),
                    new Ingredient(250, floor, g),
                    new Ingredient(3, egg, n)
                },
                Guid.NewGuid()
            );

            IKitchen kitchen = new Kitchen();

            // When
            Action act = () => kitchen.CookRecipeFromStock(recipe, new List<Stock> {milkStock,tomatoStock, floorStock, eggStock});

            
            // Then
            act.Should()
                .Throw<ArgumentException>();
        }
    }
}