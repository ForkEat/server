using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IProductRepository productRepository;
        private readonly IUnitRepository unitsRepository;
        private readonly ILikeRepository likeRepository;
        private readonly IStockRepository stockRepository;
        private readonly IKitchen kitchen;

        public RecipeService(
            IRecipeRepository recipeRepository,
            IProductRepository productRepository,
            IUnitRepository unitsRepository,
            ILikeRepository likeRepository,
            IStockRepository stockRepository,
            IKitchen kitchen)
        {
            this.recipeRepository = recipeRepository;
            this.productRepository = productRepository;
            this.unitsRepository = unitsRepository;
            this.likeRepository = likeRepository;
            this.kitchen = kitchen;
            this.stockRepository = stockRepository;
        }

        public async Task PerformRecipe(Guid recipeId)
        {
            Recipe recipe = await this.recipeRepository.GetRecipeById(recipeId);

            List<Guid> recipeIngredientsProductIds = recipe.Ingredients
                .Select(ingredient => ingredient.Product.Id)
                .ToList();

            List<Stock> stocks = await this.stockRepository.FindAllStocksByProductIds(recipeIngredientsProductIds);
            this.kitchen.CookRecipeFromStock(recipe, stocks);

            foreach (var stock in stocks)
            {
                await this.stockRepository.UpdateStock(stock);
            }
        }

        public async Task<GetRecipeWithStepsAndIngredientsResponse> CreateRecipe(CreateRecipeRequest request)
        {
            var products = await productRepository.FindProductsByIds(ExtractProductIds(request));
            var units = await unitsRepository.FindUnitsByIds(ExtractUnitIds(request));
            var recipe = BuildRecipeFromRequest(request, products, units);

            recipe = await this.recipeRepository.InsertRecipe(recipe);

            return new GetRecipeWithStepsAndIngredientsResponse(recipe);
        }

        private List<Guid> ExtractUnitIds(CreateRecipeRequest request)
        {
            return request.Ingredients.Select(i => i.UnitId).ToList();
        }

        public async Task<IList<GetRecipesResponse>> GetRecipes(Guid userId)
        {
            var recipes = await recipeRepository.GetAllRecipes();
            var likes = await likeRepository.GetLikes(userId, recipes.Select(recipe => recipe.Id).ToList());

            return recipes
                    .Select(recipe =>
                    {
                        var response = new GetRecipesResponse(recipe);
                        response.IsLiked = likes.Contains(recipe.Id);
                        return response;
                    })
                    .ToList();
        }

        public async Task<GetRecipeWithStepsAndIngredientsResponse> GetRecipeById(Guid recipeId, Guid userId)
        {
            var recipe = await recipeRepository.GetRecipeById(recipeId);
            var response = new GetRecipeWithStepsAndIngredientsResponse(recipe)
            {
                IsLiked = await likeRepository.GetLike(userId, recipeId)
            };
            return response;
        }

        private static Recipe BuildRecipeFromRequest(
            CreateRecipeRequest request, Dictionary<Guid, Product> products,
            Dictionary<Guid, Unit> dictionary)
        {
            return new Recipe(
                request is UpdateRecipeRequest updateRecipeRequest ? updateRecipeRequest.Id : Guid.NewGuid(),
                request.Name,
                request.Difficulty,
                request.Steps.Select(stepRequest => new Step(
                    Guid.NewGuid(),
                    stepRequest.Name,
                    stepRequest.Instructions,
                    TimeSpan.FromSeconds(stepRequest.EstimatedTime))
                ).ToList(),
                request.Ingredients.Select(i => new Ingredient(i.Quantity, products[i.ProductId], dictionary[i.UnitId])).ToList(),
                request.ImageId);

        }

        private static List<Guid> ExtractProductIds(CreateRecipeRequest request)
        {
            return request.Ingredients.Select(i => i.ProductId).ToList();
        }

        public Task DeleteRecipeById(Guid recipeId) => this.recipeRepository.DeleteRecipeById(recipeId);

        public async Task<GetRecipeWithStepsAndIngredientsResponse> UpdateRecipe(Guid recipeId,
            UpdateRecipeRequest request)
        {
            await this.recipeRepository.DeleteRecipeById(recipeId);
            var products = await productRepository.FindProductsByIds(ExtractProductIds(request));
            var units = await unitsRepository.FindUnitsByIds(ExtractUnitIds(request));
            var recipe = BuildRecipeFromRequest(request, products, units);

            recipe = await this.recipeRepository.InsertRecipe(recipe);

            return new GetRecipeWithStepsAndIngredientsResponse(recipe);
        }

        public async Task<IList<GetRecipesResponse>> SearchRecipeByIngredients(IList<Guid> guids)
        {
            var recipes = await this.recipeRepository.FindRecipesWithIngredients(guids);
            return recipes
                .Select(recipe => new GetRecipesResponse(recipe))
                .ToList();
        }

        public async Task<bool> LikeRecipe(Guid userId, Guid recipeId)
        {
            var recipe = await recipeRepository.GetRecipeById(recipeId);
            if (recipe == null)
            {
                throw new RecipeNotFoundException();
            }

            if (await likeRepository.GetLike(userId, recipeId))
            {
                return true;
            }

            return await likeRepository.LikeRecipe(userId, recipeId);
        }

        public async Task UnlikeRecipe(Guid userId, Guid recipeId)
        {
            var recipe = await recipeRepository.GetRecipeById(recipeId);
            if (recipe == null)
            {
                throw new RecipeNotFoundException();
            }

            if (!await likeRepository.GetLike(userId, recipeId))
            {
                return;
            }

            await likeRepository.UnlikeRecipe(userId, recipeId);
        }
    }
}