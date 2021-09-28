using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository recipeRepository;
        private readonly IProductRepository productRepository;

        public RecipeService(IRecipeRepository recipeRepository, IProductRepository productRepository)
        {
            this.recipeRepository = recipeRepository;
            this.productRepository = productRepository;
        }

        public async Task<GetRecipeWithStepsAndIngredientsResponse> CreateRecipe(CreateRecipeRequest request)
        {
            var products = await productRepository.FindProductsByIds(ExtractProductIds(request));
            var recipe = BuildRecipeFromRequest(request, products);

            recipe = await this.recipeRepository.InsertRecipe(recipe);

            return new GetRecipeWithStepsAndIngredientsResponse(recipe);
        }

        public async Task<List<GetRecipesResponse>> GetRecipes()
        {
            var recipes = await recipeRepository.GetAllRecipes();
            
            return recipes
                    .Select(recipe => new GetRecipesResponse(recipe))
                    .ToList()
;        }

        private static Recipe BuildRecipeFromRequest(CreateRecipeRequest request, Dictionary<Guid, Product> products)
        {
            return new Recipe(
                Guid.NewGuid(),
                request.Name,
                request.Difficulty,
                request.Steps.Select(stepRequest => new Step(
                    Guid.NewGuid(),
                    stepRequest.Name,
                    stepRequest.Instructions,
                    stepRequest.EstimatedTime)
                ).ToList(),
                request.Ingredients.Select(i => new Ingredient(products[i.ProductId], i.Quantity)).ToList());
        }

        private static List<Guid> ExtractProductIds(CreateRecipeRequest request)
        {
            return request.Ingredients.Select(i => i.ProductId).ToList();
        }
    }
}