using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using Moq;
using Xunit;

namespace ForkEat.Core.Tests.Services
{
    public class RecipeServiceTest
    {
        [Fact]
        public async Task CreateRecipe_InsertRecipeInRepo()
        {
            // Given
            var product1 = new Product() { Id = Guid.NewGuid(), Name = "Product 1" };
            var product2 = new Product() { Id = Guid.NewGuid(), Name = "Product 2" };

            var recipeRequest = new CreateRecipeRequest()
            {
                Name = "Test Recipe",
                Difficulty = 3,
                Steps = new List<CreateStepRequest>
                {
                    new CreateStepRequest()
                    {
                        Name = "Test Step 1", Instructions = "Test Step 1 instructions",
                        EstimatedTime = new TimeSpan(0, 1, 30)
                    },
                    new CreateStepRequest()
                    {
                        Name = "Test Step 2", Instructions = "Test Step 2 instructions",
                        EstimatedTime = new TimeSpan(0, 2, 0)
                    },
                    new CreateStepRequest()
                    {
                        Name = "Test Step 3", Instructions = "Test Step 3 instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    },
                },
                Ingredients = new List<CreateIngredientRequest>()
                {
                    new CreateIngredientRequest() { ProductId = product1.Id, Quantity = 1 },
                    new CreateIngredientRequest() { ProductId = product2.Id, Quantity = 2 }
                }
            };

            Recipe insertedRecipe = null;
            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock.Setup(mock => mock.InsertRecipe(It.IsAny<Recipe>()))
                .Returns<Recipe>(r =>
                {
                    insertedRecipe = r;
                    return Task.FromResult(r);
                });

            var repoProductMock = new Mock<IProductRepository>();
            repoProductMock.Setup(mock => mock.FindProductsByIds(It.IsAny<List<Guid>>()))
                .Returns<List<Guid>>(ids => Task.FromResult(new Dictionary<Guid, Product>()
                {
                    [product1.Id] = product1,
                    [product2.Id] = product2,
                }));

            var service = new RecipeService(repoRecipeMock.Object, repoProductMock.Object);

            // When
            var result = await service.CreateRecipe(recipeRequest);

            // Then
            insertedRecipe.Id.Should().NotBeEmpty();
            insertedRecipe.Name.Should().Be("Test Recipe");
            insertedRecipe.Difficulty.Should().Be(3);
            insertedRecipe.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 30));
            insertedRecipe.Steps.Should().HaveCount(3);


            insertedRecipe.Steps[0].Id.Should().NotBeEmpty();
            insertedRecipe.Steps[0].Name.Should().Be("Test Step 1");
            insertedRecipe.Steps[0].Instructions.Should().Be("Test Step 1 instructions");
            insertedRecipe.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 30));

            insertedRecipe.Steps[1].Id.Should().NotBeEmpty();
            insertedRecipe.Steps[1].Name.Should().Be("Test Step 2");
            insertedRecipe.Steps[1].Instructions.Should().Be("Test Step 2 instructions");
            insertedRecipe.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 2, 0));

            insertedRecipe.Steps[2].Id.Should().NotBeEmpty();
            insertedRecipe.Steps[2].Name.Should().Be("Test Step 3");
            insertedRecipe.Steps[2].Instructions.Should().Be("Test Step 3 instructions");
            insertedRecipe.Steps[2].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            insertedRecipe.Ingredients.Should().HaveCount(2);

            insertedRecipe.Ingredients[0].Product.Id.Should().Be(product1.Id);
            insertedRecipe.Ingredients[0].Quantity.Should().Be(1);

            insertedRecipe.Ingredients[1].Product.Id.Should().Be(product2.Id);
            insertedRecipe.Ingredients[1].Quantity.Should().Be(2);

            result.Id.Should().Be(insertedRecipe.Id);
            result.Name.Should().Be("Test Recipe");
            result.Difficulty.Should().Be(3);
            result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 30));
            result.Steps.Should().HaveCount(3);

            result.Steps[0].Name.Should().Be("Test Step 1");
            result.Steps[0].Instructions.Should().Be("Test Step 1 instructions");
            result.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 30));

            result.Steps[1].Name.Should().Be("Test Step 2");
            result.Steps[1].Instructions.Should().Be("Test Step 2 instructions");
            result.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 2, 0));

            result.Steps[2].Name.Should().Be("Test Step 3");
            result.Steps[2].Instructions.Should().Be("Test Step 3 instructions");
            result.Steps[2].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));

            result.Ingredients.Should().HaveCount(2);

            var ingredient1Response = result.Ingredients.First(i => i.ProductId == product1.Id);
            ingredient1Response.Quantity.Should().Be(1);
            ingredient1Response.Name.Should().Be("Product 1");

            var ingredient2Response = result.Ingredients.First(i => i.ProductId == product2.Id);
            ingredient2Response.Quantity.Should().Be(2);
            ingredient2Response.Name.Should().Be("Product 2");
        }

        [Fact]
        public async Task GetRecipes_ReturnsAllRecipesInRepo()
        {
            // Given
            var product1 = new Product() { Id = Guid.NewGuid(), Name = "Product 1" };
            var product2 = new Product() { Id = Guid.NewGuid(), Name = "Product 2" };


            var recipe1 = new Recipe(Guid.NewGuid(), "Test Recipe 1", 1, new List<Step>()
            {
                new Step(Guid.NewGuid(), "Test Step 1", "Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                new Step(Guid.NewGuid(), "Test Step 2", "Test Step 2 Instructions", new TimeSpan(0, 1, 0)),
                new Step(Guid.NewGuid(), "Test Step 3", "Test Step 3 Instructions", new TimeSpan(0, 1, 30))
            }, new List<Ingredient>()
            {
                new Ingredient(product1, 1),
                new Ingredient(product2, 2)
            });
            
            var recipe2 = new Recipe(Guid.NewGuid(), "Test Recipe 2", 1, new List<Step>()
            {
                new Step(Guid.NewGuid(), "Test Step 4", "Test Step 4 Instructions", new TimeSpan(0, 1, 30)),
                new Step(Guid.NewGuid(), "Test Step 5", "Test Step 5 Instructions", new TimeSpan(0, 1, 0)),
                new Step(Guid.NewGuid(), "Test Step 3", "Test Step 6 Instructions", new TimeSpan(0, 1, 30))
            }, new List<Ingredient>()
            {
                new Ingredient(product1, 1),
                new Ingredient(product2, 2)
            });

            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock
                .Setup(mock => mock.GetAllRecipes())
                .Returns(() => Task.FromResult(new List<Recipe>() { recipe1,recipe2 }));

            var service = new RecipeService(repoRecipeMock.Object, null);

            // When
            var result = await service.GetRecipes();

            // Then
            result.Should().HaveCount(2);
            var recipe1Result = result.First(recipe => recipe.Id == recipe1.Id);
            var recipe2Result = result.First(recipe => recipe.Id == recipe2.Id);

            recipe1Result.Id.Should().Be(recipe1.Id);
            recipe1Result.Name.Should().Be("Test Recipe 1");
            recipe1Result.Difficulty.Should().Be(1);
            recipe1Result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 0));
            
            recipe2Result.Id.Should().Be(recipe2.Id);
            recipe2Result.Name.Should().Be("Test Recipe 2");
            recipe2Result.Difficulty.Should().Be(1);
            recipe2Result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 0));

        }
        
                [Fact]
        public async Task GetRecipeById_ReturnsRecipeByIdFromRepo()
        {
            // Given
            var product1 = new Product() { Id = Guid.NewGuid(), Name = "Product 1" };
            var product2 = new Product() { Id = Guid.NewGuid(), Name = "Product 2" };


            var recipe1 = new Recipe(Guid.NewGuid(), "Test Recipe 1", 1, new List<Step>()
            {
                new Step(Guid.NewGuid(), "Test Step 1", "Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                new Step(Guid.NewGuid(), "Test Step 2", "Test Step 2 Instructions", new TimeSpan(0, 1, 0)),
                new Step(Guid.NewGuid(), "Test Step 3", "Test Step 3 Instructions", new TimeSpan(0, 1, 30))
            }, new List<Ingredient>()
            {
                new Ingredient(product1, 1),
                new Ingredient(product2, 2)
            });
            

            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock
                .Setup(mock => mock.GetRecipeById(recipe1.Id))
                .Returns(() => Task.FromResult(recipe1));

            var service = new RecipeService(repoRecipeMock.Object, null);

            // When
            var result = await service.GetRecipeById(recipe1.Id);

            // Then

            result.Id.Should().Be(recipe1.Id);
            result.Name.Should().Be("Test Recipe 1");
            result.Difficulty.Should().Be(1);
            result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 0));

            result.Ingredients.Should().HaveCount(2);
            result.Ingredients[0].Name.Should().Be("Product 1");
            result.Ingredients[0].Quantity.Should().Be(1);
            result.Ingredients[0].ProductId.Should().Be(product1.Id);
            result.Ingredients[1].Name.Should().Be("Product 2");
            result.Ingredients[1].Quantity.Should().Be(2);
            result.Ingredients[1].ProductId.Should().Be(product2.Id);

            result.Steps.Should().HaveCount(3);
            result.Steps[0].Name.Should().Be("Test Step 1");
            result.Steps[0].Instructions.Should().Be("Test Step 1 Instructions");
            result.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 30));
            result.Steps[1].Name.Should().Be("Test Step 2");
            result.Steps[1].Instructions.Should().Be("Test Step 2 Instructions");
            result.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 1, 0));
            result.Steps[2].Name.Should().Be("Test Step 3");
            result.Steps[2].Instructions.Should().Be("Test Step 3 Instructions");
            result.Steps[2].EstimatedTime.Should().Be(new TimeSpan(0, 1, 30));
        }

        [Fact]
        public async Task DeleteRecipe_DeletesRecipeInRepo()
        {
            // Given
            var recipeId = Guid.NewGuid();
            
            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock.Setup(mock => mock.DeleteRecipeById(recipeId));
            
            var service = new RecipeService(repoRecipeMock.Object, null);
            
            // When
            await service.DeleteRecipeById(recipeId);
            
            // Then
            repoRecipeMock.Verify(mock => mock.DeleteRecipeById(recipeId), Times.Once);
        }
    }
}