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
                }
            };

            Recipe insertedRecipe = null;
            var repoMock = new Mock<IRecipeRepository>();
            repoMock.Setup(mock => mock.InsertRecipe(It.IsAny<Recipe>()))
                .Returns<Recipe>(r =>
                {
                    insertedRecipe = r;
                    return Task.FromResult(r);
                });

            var service = new RecipeService(repoMock.Object);

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
            
            result.Id.Should().Be(insertedRecipe.Id);
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
        }
    }
}