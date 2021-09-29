using System;
using System.Collections.Generic;
using FluentAssertions;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests.Domain
{
    public class RecipeTest
    {
        [Fact]
        public void TotalEstimatedTime_ComputedFromSteps()
        {
            // Given
            var recipe = new Recipe(
                Guid.NewGuid(),
                "Test Recipe",
                3,
                new List<Step>
                {
                    new Step(Guid.NewGuid(),"Test Step 1 Name","Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                    new Step(Guid.NewGuid(),"Test Step 1 Name","Test Step 1 Instructions", new TimeSpan(0, 1, 0) ),
                    new Step(Guid.NewGuid(),"Test Step 1 Name","Test Step 1 Instructions", new TimeSpan(0, 3, 0) )
                },new List<Ingredient>(),
                Guid.NewGuid()
            );

            // When
            TimeSpan totalEstimatedTime = recipe.TotalEstimatedTime;

            // Then
            totalEstimatedTime.Should().Be(new TimeSpan(0, 5, 30));
        }

        [Fact]
        public void Difficulty_IsBetween0And5_NoError()
        {
            // Given
            var recipe = new Recipe(Guid.NewGuid(),
                "Test Recipe", 3, new List<Step>(),new List<Ingredient>(), Guid.NewGuid());

            // When
            recipe.Invoking(r => r.Difficulty = 3)

                // Then
                .Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void Difficulty_Above5_ThrowsException()
        {
            // Given
            var recipe = new Recipe(Guid.NewGuid(),
                "Test Recipe", 3, new List<Step>(),new List<Ingredient>(), Guid.NewGuid());

            // When
            recipe.Invoking(r => r.Difficulty = 6)

                // Then
                .Should().Throw<ArgumentException>()
                .WithMessage("Recipe difficulty should be between 0 and 5");
        }
        
        [Fact]
        public void Name_NotNull()
        {
            // Given
            var recipe = new Recipe(Guid.NewGuid(),
                "Test Recipe", 3, new List<Step>(), new List<Ingredient>(), Guid.Empty);

            // When
            recipe.Invoking(r => r.Name = null)

                // Then
                .Should().Throw<ArgumentException>()
                .WithMessage("Invalid recipe name (null or empty)");
        }
        
        [Fact]
        public void Name_NotBlank()
        {
            // Given
            var recipe = new Recipe(Guid.NewGuid(),
                "Test Recipe", 3, new List<Step>(), new List<Ingredient>(), Guid.Empty);

            // When
            recipe.Invoking(r => r.Name = "")

                // Then
                .Should().Throw<ArgumentException>()
                .WithMessage("Invalid recipe name (null or empty)");
        }
    }
}