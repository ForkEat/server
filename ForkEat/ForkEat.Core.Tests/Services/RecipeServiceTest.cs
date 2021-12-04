using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
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
            var (product1, product2) = CreateProducts();

            var imageId = Guid.NewGuid();
            var unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};


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
                    new CreateOrUpdateIngredientRequest() {Quantity = 1, ProductId = product1.Id, UnitId = unit.Id},
                    new CreateOrUpdateIngredientRequest() {Quantity = 2, ProductId = product2.Id, UnitId = unit.Id}
                },
                ImageId = imageId
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

            var repoUnitMock = new Mock<IUnitRepository>();
            repoUnitMock.Setup(mock => mock.FindUnitsByIds(It.IsAny<List<Guid>>()))
                .Returns<List<Guid>>(ids => Task.FromResult(new Dictionary<Guid, Unit>()
                {
                    [unit.Id] = unit,
                }));

            var service = new RecipeService(repoRecipeMock.Object, repoProductMock.Object, repoUnitMock.Object, null,
                null, null);

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
            insertedRecipe.Ingredients[0].Unit.Id.Should().Be(unit.Id);
            insertedRecipe.Ingredients[0].Unit.Name.Should().Be("Kilogramme");
            insertedRecipe.Ingredients[0].Unit.Symbol.Should().Be("kg");

            insertedRecipe.Ingredients[1].Product.Id.Should().Be(product2.Id);
            insertedRecipe.Ingredients[1].Quantity.Should().Be(2);
            insertedRecipe.Ingredients[1].Unit.Id.Should().Be(unit.Id);
            insertedRecipe.Ingredients[1].Unit.Name.Should().Be("Kilogramme");
            insertedRecipe.Ingredients[1].Unit.Symbol.Should().Be("kg");

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
            ingredient1Response.Unit.Name.Should().Be("Kilogramme");
            ingredient1Response.Unit.Symbol.Should().Be("kg");

            var ingredient2Response = result.Ingredients.First(i => i.ProductId == product2.Id);
            ingredient2Response.Quantity.Should().Be(2);
            ingredient2Response.Name.Should().Be("Product 2");
            ingredient2Response.Unit.Name.Should().Be("Kilogramme");
            ingredient2Response.Unit.Symbol.Should().Be("kg");
        }

        [Fact]
        public async Task GetRecipes_ReturnsAllRecipesInRepo()
        {
            // Given
            var (product1, product2) = CreateProducts();
            var imageId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};

            var recipe1 = new Recipe(Guid.NewGuid(), "Test Recipe 1", 1, new List<Step>()
                {
                    new Step(Guid.NewGuid(), "Test Step 1", "Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                    new Step(Guid.NewGuid(), "Test Step 2", "Test Step 2 Instructions", new TimeSpan(0, 1, 0)),
                    new Step(Guid.NewGuid(), "Test Step 3", "Test Step 3 Instructions", new TimeSpan(0, 1, 30))
                }, new List<Ingredient>()
                {
                    new Ingredient(1, product1, unit),
                    new Ingredient(2, product2, unit)
                },
                imageId);

            var recipe2 = new Recipe(Guid.NewGuid(), "Test Recipe 2", 1, new List<Step>()
                {
                    new Step(Guid.NewGuid(), "Test Step 4", "Test Step 4 Instructions", new TimeSpan(0, 1, 30)),
                    new Step(Guid.NewGuid(), "Test Step 5", "Test Step 5 Instructions", new TimeSpan(0, 1, 0)),
                    new Step(Guid.NewGuid(), "Test Step 3", "Test Step 6 Instructions", new TimeSpan(0, 1, 30))
                }, new List<Ingredient>()
                {
                    new Ingredient(1, product1, unit),
                    new Ingredient(2, product2, unit)
                },
                imageId);


            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock
                .Setup(mock => mock.GetAllRecipes())
                .Returns(() => Task.FromResult(new List<Recipe>() {recipe1, recipe2}));

            var repoLikeMock = new Mock<ILikeRepository>();
            repoLikeMock
                .Setup(mock => mock.GetLikes(userId, new List<Guid>
                {
                    recipe1.Id,
                    recipe2.Id
                }))
                .Returns(() => Task.FromResult(new List<Guid>
                {
                    recipe1.Id
                }));

            var service = new RecipeService(repoRecipeMock.Object, null, null, repoLikeMock.Object, null, null);

            // When
            var result = await service.GetRecipes(userId);

            // Then
            result.Should().HaveCount(2);
            var recipe1Result = result.First(recipe => recipe.Id == recipe1.Id);
            var recipe2Result = result.First(recipe => recipe.Id == recipe2.Id);

            recipe1Result.Id.Should().Be(recipe1.Id);
            recipe1Result.Name.Should().Be("Test Recipe 1");
            recipe1Result.ImageId.Should().NotBe(Guid.Empty);
            recipe1Result.Difficulty.Should().Be(1);
            recipe1Result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 0));

            recipe2Result.Id.Should().Be(recipe2.Id);
            recipe2Result.Name.Should().Be("Test Recipe 2");
            recipe2Result.ImageId.Should().NotBe(Guid.Empty);
            recipe2Result.Difficulty.Should().Be(1);
            recipe2Result.TotalEstimatedTime.Should().Be(new TimeSpan(0, 4, 0));
        }

        [Fact]
        public async Task GetRecipeById_ReturnsRecipeByIdFromRepo()
        {
            // Given
            var (product1, product2) = CreateProducts();
            var userId = Guid.NewGuid();

            var unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};


            var recipe1 = new Recipe(Guid.NewGuid(), "Test Recipe 1", 1, new List<Step>()
            {
                new Step(Guid.NewGuid(), "Test Step 1", "Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                new Step(Guid.NewGuid(), "Test Step 2", "Test Step 2 Instructions", new TimeSpan(0, 1, 0)),
                new Step(Guid.NewGuid(), "Test Step 3", "Test Step 3 Instructions", new TimeSpan(0, 1, 30))
            }, new List<Ingredient>()
            {
                new Ingredient(1, product1, unit),
                new Ingredient(2, product2, unit)
            }, Guid.NewGuid());


            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock
                .Setup(mock => mock.GetRecipeById(recipe1.Id))
                .Returns(() => Task.FromResult(recipe1));

            var repoLikeMock = new Mock<ILikeRepository>();
            repoLikeMock
                .Setup(mock => mock.GetLike(userId, recipe1.Id))
                .Returns(() => Task.FromResult(true));

            var service = new RecipeService(repoRecipeMock.Object, null, null, repoLikeMock.Object, null, null);

            // When
            var result = await service.GetRecipeById(recipe1.Id, userId);

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

            var service = new RecipeService(repoRecipeMock.Object, null, null, null, null, null);

            // When
            await service.DeleteRecipeById(recipeId);

            // Then
            repoRecipeMock.Verify(mock => mock.DeleteRecipeById(recipeId), Times.Once);
        }

        [Fact]
        public async Task UpdateRecipe_DeletesThenInsertInRepo()
        {
            // Given
            var (product1, product2) = CreateProducts();

            var unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};


            var recipe1 = new Recipe(Guid.NewGuid(), "Test Recipe 1", 1, new List<Step>()
            {
                new Step(Guid.NewGuid(), "Test Step 1", "Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                new Step(Guid.NewGuid(), "Test Step 2", "Test Step 2 Instructions", new TimeSpan(0, 1, 0)),
                new Step(Guid.NewGuid(), "Test Step 3", "Test Step 3 Instructions", new TimeSpan(0, 1, 30))
            }, new List<Ingredient>()
            {
                new Ingredient(1, product1, unit),
                new Ingredient(2, product2, unit)
            }, Guid.NewGuid());

            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoRecipeMock.Setup(mock => mock.DeleteRecipeById(recipe1.Id));
            repoRecipeMock.Setup(mock => mock.InsertRecipe(It.IsAny<Recipe>())).Returns<Recipe>(Task.FromResult);
            ;

            var repoProductMock = new Mock<IProductRepository>();
            repoProductMock.Setup(mock => mock.FindProductsByIds(It.IsAny<List<Guid>>()))
                .Returns<List<Guid>>(ids => Task.FromResult(new Dictionary<Guid, Product>()
                {
                    [product1.Id] = product1,
                    [product2.Id] = product2,
                }));

            var repoUnitMock = new Mock<IUnitRepository>();
            repoUnitMock.Setup(mock => mock.FindUnitsByIds(It.IsAny<List<Guid>>()))
                .Returns<List<Guid>>(ids => Task.FromResult(new Dictionary<Guid, Unit>()
                {
                    [unit.Id] = unit,
                }));

            var service = new RecipeService(repoRecipeMock.Object, repoProductMock.Object, repoUnitMock.Object, null,
                null, null);

            // When
            GetRecipeWithStepsAndIngredientsResponse updatedRecipe = await service.UpdateRecipe(recipe1.Id,
                new UpdateRecipeRequest()
                {
                    Name = "Test Recipe 1 Updated",
                    Difficulty = 2,
                    Ingredients = new List<CreateOrUpdateIngredientRequest>()
                    {
                        new CreateOrUpdateIngredientRequest()
                            {ProductId = product1.Id, UnitId = unit.Id, Quantity = 3},
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
                });

            // Then
            repoRecipeMock.Verify(mock => mock.DeleteRecipeById(recipe1.Id), Times.Once);
            repoRecipeMock.Verify(mock => mock.InsertRecipe(It.IsAny<Recipe>()), Times.Once);
        }

        private static (Product, Product) CreateProducts()
        {
            var product1 = new Product(Guid.NewGuid(), "Product 1", Guid.NewGuid());
            var product2 = new Product(Guid.NewGuid(), "Product 2", Guid.NewGuid());
            return (product1, product2);
        }

        [Fact]
        public async Task SearchRecipeByIngredients_returnsRecipesFromRepo()
        {
            // Given
            var recipe = new Recipe(
                Guid.NewGuid(),
                "Test Recipe",
                2,
                new List<Step>(),
                new List<Ingredient>(),
                Guid.NewGuid()
            );

            IList<Guid> receivedIds = null;
            var recipeRepoMock = new Mock<IRecipeRepository>();
            recipeRepoMock.Setup(mock => mock.FindRecipesWithIngredients(It.IsAny<IList<Guid>>()))
                .Returns<IList<Guid>>(ids =>
                {
                    receivedIds = ids;
                    return Task.FromResult(new List<Recipe>() {recipe} as IList<Recipe>);
                });

            IRecipeService service = new RecipeService(recipeRepoMock.Object, null, null, null, null, null);

            // When
            var result = await service.SearchRecipeByIngredients(new List<Guid> {recipe.Id});

            // Then
            result.Should().ContainSingle();
            result[0].Id.Should().Be(recipe.Id);
        }

        [Fact]
        public async Task PerformRecipe_QueriesStockAndPassesItToKitchen()
        {
            // Given
            var (product1, product2) = CreateProducts();
            var imageId = Guid.NewGuid();

            var unit = new Unit() {Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg"};

            var recipe = new Recipe(Guid.NewGuid(), "Test Recipe 1", 1, new List<Step>()
            {
                new Step(Guid.NewGuid(), "Test Step 1", "Test Step 1 Instructions", new TimeSpan(0, 1, 30)),
                new Step(Guid.NewGuid(), "Test Step 2", "Test Step 2 Instructions", new TimeSpan(0, 1, 0)),
                new Step(Guid.NewGuid(), "Test Step 3", "Test Step 3 Instructions", new TimeSpan(0, 1, 30))
            }, new List<Ingredient>()
            {
                new Ingredient(1, product1, unit),
                new Ingredient(2, product2, unit)
            }, Guid.NewGuid());

            var stock = new List<Stock>
            {
                new Stock(2, unit, product1),
                new Stock(2, unit, product2)
            };

            var recipeRepoMock = new Mock<IRecipeRepository>();
            recipeRepoMock.Setup(mock => mock.GetRecipeById(recipe.Id))
                .Returns(() => Task.FromResult<Recipe>(recipe));

            var stockRepoMock = new Mock<IStockRepository>();
            var productIds = recipe.Ingredients.Select(i => i.Product.Id).ToList();
            stockRepoMock.Setup(mock => mock.FindAllStocksByProductIds(productIds))
                .Returns(() => Task.FromResult(stock));
            stockRepoMock.Setup(mock => mock.UpdateStock(It.IsAny<Stock>()));

            var kitchenMock = new Mock<IKitchen>();
            kitchenMock.Setup(mock => mock.CookRecipeFromStock(recipe, stock));

            IRecipeService service = new RecipeService(recipeRepoMock.Object, null, null, null, stockRepoMock.Object, kitchenMock.Object);

            // When
            await service.PerformRecipe(recipe.Id);

            // Then
            recipeRepoMock.Verify(mock => mock.GetRecipeById(recipe.Id), Times.Once);
            stockRepoMock.Verify(mock => mock.FindAllStocksByProductIds(productIds), Times.Once);
            stockRepoMock.Verify(mock => mock.UpdateStock(It.IsAny<Stock>()), Times.Exactly(2));
            kitchenMock.Verify(mock => mock.CookRecipeFromStock(recipe, stock), Times.Once);
        }

        [Fact]
        public async Task LikeRecipe_UpdateIsLiked()
        {
            //Given
            var recipeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var repoRecipeMock = new Mock<IRecipeRepository>();
            var repoLikeMock = new Mock<ILikeRepository>();
            repoLikeMock.Setup(mock => mock.LikeRecipe(userId, recipeId)).Returns<Guid, Guid>((_, _) => Task.FromResult(true));
            repoLikeMock.Setup(mock => mock.GetLike(userId, recipeId)).Returns<Guid, Guid>((_, _) => Task.FromResult(true));
            repoRecipeMock.Setup(mock => mock.GetRecipeById(recipeId)).Returns<Guid>( _ =>
                Task.FromResult(new Recipe(recipeId, "recipe", 0, new List<Step>(), new List<Ingredient>(), Guid.Empty)));

            var service = new RecipeService(repoRecipeMock.Object, null, null, repoLikeMock.Object, null, null);

            // When
            var result = await service.LikeRecipe(userId, recipeId);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public async Task LikeRecipe_WithUnExistingRecipe_ThrowsException()
        {
            //Given
            var recipeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var repoRecipeMock = new Mock<IRecipeRepository>();
            var repoLikeMock = new Mock<ILikeRepository>();
            repoRecipeMock.Setup(mock => mock.GetRecipeById(recipeId)).Returns<Guid>( _ => Task.FromResult<Recipe>(null));

            var service = new RecipeService(repoRecipeMock.Object, null, null, repoLikeMock.Object, null, null);

            // When / Then
            repoLikeMock.Verify(mock => mock.LikeRecipe(userId, recipeId), Times.Never);
            await service.Invoking(_ => service.LikeRecipe(userId, recipeId))
                .Should().ThrowAsync<RecipeNotFoundException>();
        }

        [Fact]
        public async Task UnlikeRecipe_UpdateIsUnliked()
        {
            //Given
            var recipeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var repoLikeMock = new Mock<ILikeRepository>();
            var repoRecipeMock = new Mock<IRecipeRepository>();
            repoLikeMock.Setup(mock => mock.UnlikeRecipe(userId, recipeId)).Returns<Guid, Guid>((_, _) => Task.CompletedTask);
            repoLikeMock.Setup(mock => mock.GetLike(userId, recipeId)).Returns<Guid, Guid>((_, _) => Task.FromResult<bool>(true));
            repoRecipeMock.Setup(mock => mock.GetRecipeById(recipeId)).Returns<Guid>( _ =>
                Task.FromResult(new Recipe(recipeId, "recipe", 0, new List<Step>(), new List<Ingredient>(), Guid.Empty)));
            var service = new RecipeService(repoRecipeMock.Object, null, null, repoLikeMock.Object, null, null);

            // When
            await service.UnlikeRecipe(userId, recipeId);

            // Then
            repoLikeMock.Verify(mock => mock.UnlikeRecipe(userId, recipeId), Times.Once);
        }

        [Fact]
        public async Task UnlikeRecipe_WithUnExistingRecipe_ThrowsException()
        {
            //Given
            var recipeId = Guid.NewGuid();
            var repoRecipeMock = new Mock<IRecipeRepository>();
            var repoLikeMock = new Mock<ILikeRepository>();
            repoRecipeMock.Setup(mock => mock.GetRecipeById(recipeId)).Returns<Guid>( _ => Task.FromResult<Recipe>(null));
            var service = new RecipeService(repoRecipeMock.Object, null, null, repoLikeMock.Object, null, null);
            var userId = Guid.NewGuid();

            // When / Then
            repoLikeMock.Verify(mock => mock.UnlikeRecipe(userId, recipeId), Times.Never);
            await service.Invoking(_ => service.UnlikeRecipe(userId, recipeId))
                .Should().ThrowAsync<RecipeNotFoundException>();
        }
    }
}