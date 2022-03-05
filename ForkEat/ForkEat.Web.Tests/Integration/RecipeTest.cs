using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace ForkEat.Web.Tests.Integration;

public class RecipeTest : AuthenticatedTests
{
    public RecipeTest(WebApplicationFactory<Startup> factory) : base(factory,
        new string[] {"Likes", "Stocks", "Recipes", "Steps", "Ingredients", "Products", "Units","Files","ProductTypes"})
    {
    }

    [Fact]
    public async Task CreateRecipe_WithValidData_Returns201InsertsRecipeAndImage()
    {
        // Given
        var unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};

        await context.Units.AddAsync(unit);
        await context.SaveChangesAsync();

        var (product1, product2) = await this.dataFactory.CreateAndInsertProducts();


        var recipeRequest = new CreateRecipeRequest()
        {
            Name = "Test Recipe",
            Difficulty = 3,
            Steps = new List<CreateOrUpdateStepRequest>
            {
                new CreateOrUpdateStepRequest()
                {
                    Name = "Test Step 1", Instructions = "Test Step 1 instructions",
                    EstimatedTime = 90
                },
                new CreateOrUpdateStepRequest()
                {
                    Name = "Test Step 2", Instructions = "Test Step 2 instructions",
                    EstimatedTime = 120
                },
                new CreateOrUpdateStepRequest()
                {
                    Name = "Test Step 3", Instructions = "Test Step 3 instructions",
                    EstimatedTime = 60
                },
            },
            Ingredients = new List<CreateOrUpdateIngredientRequest>()
            {
                new CreateOrUpdateIngredientRequest() {ProductId = product1.Id, Quantity = 1, UnitId = unit.Id},
                new CreateOrUpdateIngredientRequest() {ProductId = product2.Id, Quantity = 2, UnitId = unit.Id}
            }
        };

        // When
        var response = await client.PostAsync("/api/recipes", CreateRecipeRequestContent(recipeRequest));

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Test Recipe");
        result.Difficulty.Should().Be(3);
        result.TotalEstimatedTime.Should().Be(270);
        result.Steps.Should().HaveCount(3);

        result.Steps[0].Name.Should().Be("Test Step 1");
        result.Steps[0].Instructions.Should().Be("Test Step 1 instructions");
        result.Steps[0].EstimatedTime.Should().Be(90);

        result.Steps[1].Name.Should().Be("Test Step 2");
        result.Steps[1].Instructions.Should().Be("Test Step 2 instructions");
        result.Steps[1].EstimatedTime.Should().Be(120);

        result.Steps[2].Name.Should().Be("Test Step 3");
        result.Steps[2].Instructions.Should().Be("Test Step 3 instructions");
        result.Steps[2].EstimatedTime.Should().Be(60);

        result.Ingredients.Should().HaveCount(2);

        var ingredient1Response = result.Ingredients.First(i => i.ProductId == product1.Id);
        ingredient1Response.Quantity.Should().Be(1);
        ingredient1Response.Name.Should().Be("Product 1");
        ingredient1Response.Unit.Name.Should().Be("Kilogramme");
        ingredient1Response.Unit.Symbol.Should().Be("kg");

        var ingredient2Response = result.Ingredients.First(i => i.ProductId == product2.Id);
        ingredient2Response.Quantity.Should().Be(2);
        ingredient2Response.Name.Should().Be("Product 2");
        ingredient2Response.Unit.Name.Should().Be("Kilogramme");
        ingredient2Response.Unit.Symbol.Should().Be("kg");
        
        await AssertThatFileIsEquivalentToImageFromApi("TestAssets/test-file.gif", result.ImageId);
        
        await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
        RecipeEntity recipeEntityUpdated = await specialContext.Recipes.FirstAsync(record => record.Id == result.Id);
        recipeEntityUpdated.Name.Should().Be(recipeRequest.Name);
    }

    
    [Fact]
    public async Task UpdateRecipe_WithExistingProductAndNewImage_UpdatesRecipeInDbAndReplaceImage()
    {
        // Given
        var (recipeEntity1, _) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();
        await this.context.Files.AddAsync(new DbFile() {Id = recipeEntity1.ImageId, Name = "test"});
        await this.context.SaveChangesAsync();

        var updatePayload = new UpdateRecipeRequest()
        {
            Id = recipeEntity1.Id,
            Name = "Test Recipe 1 Updated",
            Difficulty = 2,
            Ingredients = new List<CreateOrUpdateIngredientRequest>()
            {
                new CreateOrUpdateIngredientRequest()
                {
                    ProductId = recipeEntity1.Ingredients[0].Product.Id,
                    UnitId = recipeEntity1.Ingredients[0].Unit.Id, Quantity = 3
                },
            },
            Steps = new List<CreateOrUpdateStepRequest>()
            {
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 150, Instructions = "Test Step 1 Instructions Updated",
                    Name = "Test Step 1 updated"
                },
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 210, Instructions = "Test Step 2 Instructions Updated",
                    Name = "Test Step 2 updated"
                },
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 270, Instructions = "Test Step 3 Instructions Updated",
                    Name = "Test Step 3 updated"
                },
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 3600, Instructions = "Test Step 4 Instructions",
                    Name = "Test Step 4"
                },
            }
        };

        // When
        var response = await this.client.PutAsync($"/api/recipes/{recipeEntity1.Id}", CreateRecipeRequestContent(updatePayload));

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();
        result.Id.Should().Be(recipeEntity1.Id);
        result.Name.Should().Be("Test Recipe 1 Updated");
        result.Difficulty.Should().Be(2);
        result.TotalEstimatedTime.Should().Be(4230);
        result.Ingredients.Should().HaveCount(1);
        result.Steps.Should().HaveCount(4);
        result.Ingredients[0].Quantity.Should().Be(3);
        
        await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
        RecipeEntity recipeEntityUpdated = await specialContext.Recipes.FirstAsync(record => record.Id == recipeEntity1.Id);
        recipeEntityUpdated.Name.Should().Be($"Test Recipe 1 Updated");
        recipeEntityUpdated.ImageId.Should().NotBe(recipeEntity1.ImageId);
        
        int filesCount = await specialContext.Files.CountAsync();
        filesCount.Should().Be(1);
    }
    
        
    [Fact]
    public async Task UpdateRecipe_WithExistingProductAndNoImage_UpdatesRecipeInDbAndReplaceImage()
    {
        // Given
        var (recipeEntity1, _) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();
        await this.context.Files.AddAsync(new DbFile() {Id = recipeEntity1.ImageId, Name = "test"});
        await this.context.SaveChangesAsync();

        var updatePayload = new UpdateRecipeRequest()
        {
            Id = recipeEntity1.Id,
            Name = "Test Recipe 1 Updated",
            Difficulty = 2,
            Ingredients = new List<CreateOrUpdateIngredientRequest>()
            {
                new CreateOrUpdateIngredientRequest()
                {
                    ProductId = recipeEntity1.Ingredients[0].Product.Id,
                    UnitId = recipeEntity1.Ingredients[0].Unit.Id, Quantity = 3
                },
            },
            Steps = new List<CreateOrUpdateStepRequest>()
            {
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 150, Instructions = "Test Step 1 Instructions Updated",
                    Name = "Test Step 1 updated"
                },
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 210, Instructions = "Test Step 2 Instructions Updated",
                    Name = "Test Step 2 updated"
                },
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 270, Instructions = "Test Step 3 Instructions Updated",
                    Name = "Test Step 3 updated"
                },
                new CreateOrUpdateStepRequest()
                {
                    EstimatedTime = 3600, Instructions = "Test Step 4 Instructions",
                    Name = "Test Step 4"
                },
            }
        };

        // When
        var response = await this.client.PutAsync($"/api/recipes/{recipeEntity1.Id}", CreateRecipeRequestContentNoImage(updatePayload));

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();
        result.Id.Should().Be(recipeEntity1.Id);
        result.Name.Should().Be("Test Recipe 1 Updated");
        result.Difficulty.Should().Be(2);
        result.TotalEstimatedTime.Should().Be(4230);
        result.Ingredients.Should().HaveCount(1);
        result.Steps.Should().HaveCount(4);
        result.Ingredients[0].Quantity.Should().Be(3);
        
        await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
        RecipeEntity recipeEntityUpdated = await specialContext.Recipes.FirstAsync(record => record.Id == recipeEntity1.Id);
        recipeEntityUpdated.Name.Should().Be($"Test Recipe 1 Updated");
        recipeEntityUpdated.ImageId.Should().NotBe(recipeEntity1.ImageId);
        
        int filesCount = await specialContext.Files.CountAsync();
        filesCount.Should().Be(1);
    }
    
    [Fact]
    public async Task CreateRecipe_InvalidArgEmptyName_Returns400()
    {
        // Given
        var recipeRequest = new CreateRecipeRequest()
        {
            Name = "",
            Difficulty = 3,
            Steps = new List<CreateOrUpdateStepRequest>(),
            Ingredients = new List<CreateOrUpdateIngredientRequest>()
        };

        // When
        var response = await client.PostAsync("/api/recipes", CreateRecipeRequestContent(recipeRequest));

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRecipes_ReturnsRecipesInDb()
    {
        // Given
        var recipeEntity1 = new RecipeEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test Recipe 1",
            ImageId = Guid.NewGuid(),
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
            }
        };

        var recipeEntity2 = new RecipeEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test Recipe 2",
            ImageId = Guid.NewGuid(),
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
            }
        };

        await this.context.Recipes.AddRangeAsync(recipeEntity1, recipeEntity2);
        await this.context.SaveChangesAsync();

        // When
        var response = await client.GetAsync("/api/recipes");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<List<GetRecipesResponse>>();
        result.Count.Should().Be(2);

        result[0].Id.Should().Be(recipeEntity1.Id);
        result[0].Name.Should().Be("Test Recipe 1");
        result[0].ImageId.Should().NotBe(Guid.Empty);
        result[0].Difficulty.Should().Be(1);
        result[0].TotalEstimatedTime.Should().Be(120);

        result[1].Id.Should().Be(recipeEntity2.Id);
        result[1].Name.Should().Be("Test Recipe 2");
        result[1].ImageId.Should().NotBe(Guid.Empty);
        result[1].Difficulty.Should().Be(1);
        result[1].TotalEstimatedTime.Should().Be(120);
    }

    [Fact]
    public async Task GetRecipesWithIngredient_ReturnsExpectedRecipes()
    {
        // Given
        var (recipeEntity1, recipeEntity2) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();

        var product = recipeEntity1.Ingredients[1].Product.Name;

        // When
        var response = await client.GetAsync($"/api/recipes?ingredients={product}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<List<GetRecipesResponse>>();
        result.Should().ContainSingle();

        result[0].Id.Should().Be(recipeEntity1.Id);
        result[0].Name.Should().Be("Test Recipe 1");
        result[0].ImageId.Should().NotBe(Guid.Empty);
        result[0].Difficulty.Should().Be(1);
        result[0].TotalEstimatedTime.Should().Be(120);
    }

    [Fact]
    public async Task GetRecipeById_GetsCompleteRecipe()
    {
        // Given
        var (recipeEntity1, _) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();

        // When
        var response = await this.client.GetAsync($"/api/recipes/{recipeEntity1.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();

        result.Id.Should().Be(recipeEntity1.Id);
        result.Difficulty.Should().Be(1);
        result.Name.Should().Be("Test Recipe 1");
        result.Ingredients.Should().HaveCount(2);
        result.Steps.Should().HaveCount(2);
        result.TotalEstimatedTime.Should().Be(120);

        result.Steps[0].Name.Should().Be("Test Step 1");
        result.Steps[0].Instructions.Should().Be("Test Step 1 Instructions");
        result.Steps[0].EstimatedTime.Should().Be(60);

        result.Steps[1].Name.Should().Be("Test Step 2");
        result.Steps[1].Instructions.Should().Be("Test Step 2 Instructions");
        result.Steps[1].EstimatedTime.Should().Be(60);

        result.Ingredients.Select(ingredient => ingredient.Name).Should().Contain("Product 1");
        result.Ingredients.Select(ingredient => ingredient.Quantity).Should().Contain(1U);
        result.Ingredients.Select(ingredient => ingredient.Unit.Id).Should().NotBeNullOrEmpty();
        result.Ingredients.Select(ingredient => ingredient.ImageId).Should().NotBeNullOrEmpty();

        result.Ingredients.Select(ingredient => ingredient.Name).Should().Contain("Product 2");
        result.Ingredients.Select(ingredient => ingredient.Quantity).Should().Contain(2U);

        result.Ingredients.Select(ingredient => ingredient.Unit.Name).Should().AllBe("Kilogramme");
        result.Ingredients.Select(ingredient => ingredient.Unit.Symbol).Should().AllBe("kg");
    }

    [Fact]
    public async Task DeleteRecipe_DeletesRecipeFromDatabaseAndDeleteImage()
    {
        // Given
        var (recipeEntity1, _) = await this.dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();
        await this.context.Files.AddAsync(new DbFile() {Id = recipeEntity1.ImageId, Name = "test"});
        await this.context.SaveChangesAsync();
        
        // When
        var response = await this.client.DeleteAsync($"/api/recipes/{recipeEntity1.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
        (await specialContext.Recipes.CountAsync()).Should().Be(1);
        (await specialContext.Files.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task LikeRecipe_UpdatesIsLiked()
    {
        // Given
        var (recipeEntity1, _) = await dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();

        // When
        var likeResponse = await client.PostAsync($"/api/recipes/{recipeEntity1.Id}/like", null!);

        // Then
        likeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await client.GetAsync($"/api/recipes/{recipeEntity1.Id}");
        var getResult = await getResponse.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();
        getResult.IsLiked.Should().BeTrue();
    }

    [Fact]
    public async Task UnlikeRecipe_UpdatesIsLiked()
    {
        // Given
        var (recipeEntity1, _) = await dataFactory.CreateAndInsertRecipesWithIngredientsAndSteps();
        await client.PostAsync($"/api/recipes/{recipeEntity1.Id}/like", null!);

        // When
        var unlikeResponse = await client.DeleteAsync($"/api/recipes/{recipeEntity1.Id}/like");

        // Then
        unlikeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/recipes/{recipeEntity1.Id}");
        var getResult = await getResponse.Content.ReadAsAsync<GetRecipeWithStepsAndIngredientsResponse>();
        getResult.IsLiked.Should().BeFalse();
    }

    [Fact]
    public async Task PerformRecipe_ConsumesStock()
    {
        // Given
        var milk = new Product("Milk", Guid.NewGuid(), null);
        var floor = new Product("Floor", Guid.NewGuid(), null);
        var egg = new Product("Eggs", Guid.NewGuid(), null);
        var milkEntity = new ProductEntity(milk);
        var floorEntity = new ProductEntity(floor);
        var eggEntity = new ProductEntity(egg);

        await this.context.Products.AddRangeAsync(milkEntity, floorEntity, eggEntity);

        var g = new Unit() {Id = Guid.NewGuid(), Name = "Gramme", Symbol = "g"};
        var L = new Unit() {Id = Guid.NewGuid(), Name = "Litre", Symbol = "L"};
        var n = new Unit() {Id = Guid.NewGuid(), Name = "Number", Symbol = ""};

        await this.context.Units.AddRangeAsync(g, L, n);

        var milkStock = new StockEntity(new Stock(1, L, milk)) {Product = milkEntity};
        var floorStock = new StockEntity(new Stock(1000, g, floor)) {Product = floorEntity};
        var eggStock = new StockEntity(new Stock(6, n, egg)) {Product = eggEntity};

        await this.context.Stocks.AddRangeAsync(milkStock, floorStock, eggStock);

        var recipe = new RecipeEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Pancakes",
            Difficulty = 1,
            Steps = new List<StepEntity>(),
            Ingredients = new List<IngredientEntity>()
            {
                new IngredientEntity()
                {
                    Id = Guid.NewGuid(), Product = milkEntity, Quantity = 0.5, Unit = L, ProductId = milkEntity.Id
                },
                new IngredientEntity()
                {
                    Id = Guid.NewGuid(), Product = floorEntity, Quantity = 250, Unit = g, ProductId = floorEntity.Id
                },
                new IngredientEntity()
                    {Id = Guid.NewGuid(), Product = eggEntity, Quantity = 3, Unit = n, ProductId = eggEntity.Id}
            },
            ImageId = Guid.NewGuid()
        };

        await this.context.Recipes.AddAsync(recipe);
        await this.context.SaveChangesAsync();

        // When
        var response = await this.client.PostAsync($"/api/recipes/{recipe.Id}/perform", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var milkStockFromDb = await this.client.GetFromJsonAsync<List<StockResponse>>($"api/products/{milkEntity.Id}/stock");
        var floorStockFromDb = await this.client.GetFromJsonAsync<List<StockResponse>>($"api/products/{floorEntity.Id}/stock");
        var eggStockFromDb = await this.client.GetFromJsonAsync<List<StockResponse>>($"api/products/{eggEntity.Id}/stock");

        milkStockFromDb.First().Quantity.Should().Be(0.5);
        floorStockFromDb.First().Quantity.Should().Be(750);
        eggStockFromDb.First().Quantity.Should().Be(3);
    }

    private static MultipartFormDataContent CreateRecipeRequestContent(CreateRecipeRequest recipeRequest)
    {
        var formDataContent = new MultipartFormDataContent(Guid.NewGuid().ToString());

        var stringContent = new StringContent(JsonConvert.SerializeObject(recipeRequest));
        formDataContent.Add(stringContent, "payload");

        var streamContent = new StreamContent(File.OpenRead("TestAssets/test-file.gif"));
        streamContent.Headers.Add("Content-Type", "application/octet-stream");

        formDataContent.Add(streamContent, "image", Path.GetFileName("TestAssets/test-file.gif"));
        return formDataContent;
    }
        
    private static MultipartFormDataContent CreateRecipeRequestContentNoImage(CreateRecipeRequest recipeRequest)
    {
        var formDataContent = new MultipartFormDataContent(Guid.NewGuid().ToString());

        var stringContent = new StringContent(JsonConvert.SerializeObject(recipeRequest));
        formDataContent.Add(stringContent, "payload");
        
        return formDataContent;
    }
}