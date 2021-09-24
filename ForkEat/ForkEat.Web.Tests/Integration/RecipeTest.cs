using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ForkEat.Web.Tests
{
    public class RecipeTest : IntegrationTest
    {
        public RecipeTest(WebApplicationFactory<Startup> factory) : base(factory,
            new string[] { "Recipes", "StepEntity", "IngredientEntity" })
        {
        }

        [Fact]
        public async Task CreateRecipe_Returns201()
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

            await this.context.Products.AddRangeAsync(new Product[] { product1, product2 });
            await this.context.SaveChangesAsync();

            // When
            var response = await client.PostAsJsonAsync("/api/recipes", recipeRequest);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();

            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be("Test Recipe");
            result.Difficulty.Should().Be(3);
            result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 30));
            result.Steps.Should().HaveCount(3);

            result.Steps[0].Id.Should().NotBeEmpty();
            result.Steps[0].Name.Should().Be("Test Step 1");
            result.Steps[0].Instructions.Should().Be("Test Step 1 instructions");
            result.Steps[0].EstimatedTime.Should().Be(new TimeSpan(0, 1, 30));

            result.Steps[1].Id.Should().NotBeEmpty();
            result.Steps[1].Name.Should().Be("Test Step 2");
            result.Steps[1].Instructions.Should().Be("Test Step 2 instructions");
            result.Steps[1].EstimatedTime.Should().Be(new TimeSpan(0, 2, 0));

            result.Steps[2].Id.Should().NotBeEmpty();
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
        public async Task CreateRecipe_InvalidArgEmptyName_Returns400()
        {
            // Given
            var recipeRequest = new CreateRecipeRequest()
            {
                Name = "",
                Difficulty = 3,
                Steps = new List<CreateStepRequest>(),
                Ingredients = new List<CreateIngredientRequest>()
            };

            // When
            var response = await client.PostAsJsonAsync("/api/recipes", recipeRequest);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}