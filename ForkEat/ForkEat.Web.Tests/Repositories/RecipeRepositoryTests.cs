using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database.Entities;
using ForkEat.Web.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class RecipeRepositoryTests : RepositoryTest
    {
        public RecipeRepositoryTests() : base(new string[] { "Recipes", "Steps", "Ingredients","Products","Units" })
        {
        }

        [Fact]
        public async Task InsertRecipe_InsertsInDb()
        {
            // Given
            var unit = new Unit(){Id = Guid.NewGuid(), Name = "Kilogramme",Symbol = "kg"};
            var recipe = new Recipe(
                Guid.NewGuid(),
                "Test Name",
                3,
                new List<Step>()
                {
                    new Step(Guid.NewGuid(), "Test Step 1", "Test Instructions 1", new TimeSpan(0, 1, 0)),
                    new Step(Guid.NewGuid(), "Test Step 2", "Test Instructions 2", new TimeSpan(0, 1, 0)),
                },
                new List<Ingredient>()
                {
                    new Ingredient(1,new Product() { Id = Guid.NewGuid(), Name = "Test ingredient" }, unit)
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
            recipeInDb.Steps.Should().HaveCount(2);
            recipeInDb.ImageId.Should().NotBe(Guid.Empty);

            recipeInDb.Ingredients.Should().HaveCount(1);

            recipeInDb.Steps[0].Id.Should().Be(recipe.Steps[0].Id);
            recipeInDb.Steps[0].Name.Should().Be("Test Step 1");
            recipeInDb.Steps[0].Instructions.Should().Be("Test Instructions 1");
            recipeInDb.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));
            recipeInDb.Steps[0].Order.Should().Be(0);
            recipeInDb.Steps[1].Id.Should().Be(recipe.Steps[1].Id);
            recipeInDb.Steps[1].Name.Should().Be("Test Step 2");
            recipeInDb.Steps[1].Instructions.Should().Be("Test Instructions 2");
            recipeInDb.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));
            recipeInDb.Steps[1].Order.Should().Be(1);

            recipeInDb.Ingredients[0].Product.Id.Should().Be(recipe.Ingredients[0].Product.Id);
            recipeInDb.Ingredients[0].Product.Name.Should().Be(recipe.Ingredients[0].Product.Name);
            recipeInDb.Ingredients[0].Quantity.Should().Be(1);
            recipeInDb.Ingredients[0].Unit.Id.Should().Be(unit.Id);
            recipeInDb.Ingredients[0].Unit.Name.Should().Be("Kilogramme");
            recipeInDb.Ingredients[0].Unit.Symbol.Should().Be("kg");

            result.Id.Should().Be(recipe.Id);
            result.Difficulty.Should().Be(3);
            result.Name.Should().Be("Test Name");
            result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 2, 0));
            result.Steps.Should().HaveCount(2);
            result.Ingredients.Should().HaveCount(1);

            result.Steps[0].Id.Should().Be(recipe.Steps[0].Id);
            result.Steps[0].Name.Should().Be("Test Step 1");
            result.Steps[0].Instructions.Should().Be("Test Instructions 1");
            result.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));
            
            result.Steps[1].Id.Should().Be(recipe.Steps[1].Id);
            result.Steps[1].Name.Should().Be("Test Step 2");
            result.Steps[1].Instructions.Should().Be("Test Instructions 2");
            result.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            result.Ingredients[0].Product.Id.Should().Be(recipe.Ingredients[0].Product.Id);
            result.Ingredients[0].Product.Name.Should().Be(recipe.Ingredients[0].Product.Name);
            result.Ingredients[0].Quantity.Should().Be(1);
            result.Ingredients[0].Unit.Id.Should().Be(unit.Id);
            result.Ingredients[0].Unit.Name.Should().Be("Kilogramme");
            result.Ingredients[0].Unit.Symbol.Should().Be("kg");
        }

        [Fact]
        public async Task GetRecipes_RetrievesAllRecipesFromDb()
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
                        EstimatedTime = new TimeSpan(0, 1, 0),
                        Order = 0
                    },
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 2", Instructions = "Test Step 2 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0),
                        Order = 1
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
                        EstimatedTime = new TimeSpan(0, 1, 0),
                        Order = 0
                    },
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 4", Instructions = "Test Step 4 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0),
                        Order = 1
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

        [Fact]
        public async Task GetRecipeById_RetrievesRecipeWithGivenId()
        {
            // Given
            var (recipeEntity1, _) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();
            var repository = new RecipeRepository(this.context);

            // When
            Recipe result = await repository.GetRecipeById(recipeEntity1.Id);

            // Then
            result.Id.Should().Be(recipeEntity1.Id);
            result.Difficulty.Should().Be(1);
            result.Name.Should().Be("Test Recipe 1");
            result.Ingredients.Should().HaveCount(2);
            result.Steps.Should().HaveCount(2);
            result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 2, 0));

            result.Steps[0].Name.Should().Be("Test Step 1");
            result.Steps[0].Instructions.Should().Be("Test Step 1 Instructions");
            result.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            result.Steps[1].Name.Should().Be("Test Step 2");
            result.Steps[1].Instructions.Should().Be("Test Step 2 Instructions");
            result.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            result.Ingredients[0].Product.Name.Should().Be("Test Product 1");
            result.Ingredients[0].Quantity.Should().Be(1);
            result.Ingredients[0].Unit.Id.Should().Be(recipeEntity1.Ingredients[0].Unit.Id);
            result.Ingredients[0].Unit.Name.Should().Be("Kilogramme");
            result.Ingredients[0].Unit.Symbol.Should().Be("kg");
            result.Ingredients[1].Product.Name.Should().Be("Test Product 2");
            result.Ingredients[1].Quantity.Should().Be(2);
            result.Ingredients[1].Unit.Id.Should().Be(recipeEntity1.Ingredients[0].Unit.Id);
            result.Ingredients[1].Unit.Name.Should().Be("Kilogramme");
            result.Ingredients[1].Unit.Symbol.Should().Be("kg");
        }

        [Fact]
        public async Task DeleteRecipeById_DeletesRecipeWithGivenId()
        {
            // Given
            var (recipeEntity1, recipeEntity2) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();

            var repository = new RecipeRepository(this.context);

            // When
            await repository.DeleteRecipeById(recipeEntity1.Id);

            // Then
            this.context.Products.Should().HaveCount(2);
            this.context.Units.Should().HaveCount(1);
            this.context.Recipes.Should().ContainSingle();
            this.context.Ingredients.Should().HaveCount(1);
            this.context.Steps.Should().HaveCount(2);

            recipeEntity2 = await this.context.Recipes.FirstAsync();
            recipeEntity2.Name.Should().Be("Test Recipe 2");
        }

        [Fact]
        public async Task FindRecipesWithIngredients_returnsOnlyExpectedRecipe()
        {
            // Given
            var (recipeEntity1, _) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();
            var repository = new RecipeRepository(this.context);
            
            // When
            var result =
                await repository.FindRecipesWithIngredients(new List<Guid> { recipeEntity1.Ingredients[1].Product.Id });

            // Then
            result.Should().ContainSingle();
            result[0].Id.Should().Be(recipeEntity1.Id);
            result[0].Steps.Should().HaveCount(2);
        }
    }
}