﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class RecipeRepositoryTests : RepositoryTest
    {
        public RecipeRepositoryTests() : base(new string[] { "Recipes", "StepEntity", "IngredientEntity" })
        {
        }

        [Fact]
        public async Task InsertRecipe()
        {
            // Given
            var recipe = new Recipe(
                Guid.NewGuid(),
                "Test Name",
                3,
                new List<Step>()
                {
                    new Step(Guid.NewGuid(), "Test Step", "Test Instructions", new TimeSpan(0, 1, 0))
                },
                new List<Ingredient>()
                {
                    new Ingredient(new Product() { Id = Guid.NewGuid(), Name = "Test ingredient" }, 1)
                },
                Guid.NewGuid()
            );

            var repository = new RecipeRepository(this.context);

            // When
            var result = await repository.InsertRecipe(recipe);

            // Then
            var count = await this.context.Recipes.CountAsync();
            count.Should().Be(1);

            var recipeInDb = await this.context.Recipes.FirstAsync(r => r.Id == recipe.Id);
            recipeInDb.Id.Should().Be(recipe.Id);
            recipeInDb.Difficulty.Should().Be(3);
            recipeInDb.Name.Should().Be("Test Name");
            recipeInDb.Steps.Should().HaveCount(1);
            recipeInDb.ImageId.Should().NotBe(Guid.Empty);
            recipeInDb.Ingredients.Should().HaveCount(1);

            recipeInDb.Steps[0].Id.Should().Be(recipe.Steps[0].Id);
            recipeInDb.Steps[0].Name.Should().Be("Test Step");
            recipeInDb.Steps[0].Instructions.Should().Be("Test Instructions");
            recipeInDb.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            recipeInDb.Ingredients[0].Product.Id.Should().Be(recipe.Ingredients[0].Product.Id);
            recipeInDb.Ingredients[0].Product.Name.Should().Be(recipe.Ingredients[0].Product.Name);
            recipeInDb.Ingredients[0].Quantity.Should().Be(1);

            result.Id.Should().Be(recipe.Id);
            result.Difficulty.Should().Be(3);
            result.Name.Should().Be("Test Name");
            result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 1, 0));
            result.Steps.Should().HaveCount(1);
            result.Ingredients.Should().HaveCount(1);

            result.Steps[0].Id.Should().Be(recipe.Steps[0].Id);
            result.Steps[0].Name.Should().Be("Test Step");
            result.Steps[0].Instructions.Should().Be("Test Instructions");
            result.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            result.Ingredients[0].Product.Id.Should().Be(recipe.Ingredients[0].Product.Id);
            result.Ingredients[0].Product.Name.Should().Be(recipe.Ingredients[0].Product.Name);
            result.Ingredients[0].Quantity.Should().Be(1);
        }

        [Fact]
        public async Task GetRecipes()
        {
            // Given
            var imageId = Guid.NewGuid();
            var recipeEntity1 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 1",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>() { },
                Steps = new List<StepEntity>()
                {
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 1", Instructions = "Test Step 1 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    },
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 2", Instructions = "Test Step 2 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    }
                },
                ImageId = imageId
            };

            var recipeEntity2 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 2",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>() { },
                Steps = new List<StepEntity>()
                {
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 3", Instructions = "Test Step 3 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    },
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 4", Instructions = "Test Step 4 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    }
                },
                ImageId = imageId
            };

            await this.context.Recipes.AddRangeAsync(recipeEntity1, recipeEntity2);
            await this.context.SaveChangesAsync();

            var repository = new RecipeRepository(this.context);

            // When
            var result = await repository.GetAllRecipes();

            // Then
            result.Should().HaveCount(2);

            result[0].Id.Should().Be(recipeEntity1.Id);
            result[0].Name.Should().Be("Test Recipe 1");
            result[0].Difficulty.Should().Be(1);
            result[0].ImageId.Should().NotBe(Guid.Empty);
            result[0].TotalEstimatedTime.Should().Be(new TimeSpan(0, 2, 0));
            
            result[1].Id.Should().Be(recipeEntity2.Id);
            result[1].Name.Should().Be("Test Recipe 2");
            result[1].Difficulty.Should().Be(1);
            result[1].ImageId.Should().NotBe(Guid.Empty);
            result[1].TotalEstimatedTime.Should().Be(new TimeSpan(0, 2, 0));
        }
    }
}