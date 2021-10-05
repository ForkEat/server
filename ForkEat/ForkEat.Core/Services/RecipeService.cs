﻿using System;
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
        private readonly IUnitRepository unitsRepository;

        public RecipeService(IRecipeRepository recipeRepository, IProductRepository productRepository,
            IUnitRepository unitsRepository)
        {
            this.recipeRepository = recipeRepository;
            this.productRepository = productRepository;
            this.unitsRepository = unitsRepository;
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

        public async Task<List<GetRecipesResponse>> GetRecipes()
        {
            var recipes = await recipeRepository.GetAllRecipes();

            return recipes
                    .Select(recipe => new GetRecipesResponse(recipe))
                    .ToList()
                ;
        }

        public async Task<GetRecipeWithStepsAndIngredientsResponse> GetRecipeById(Guid recipeId)
        {
            var recipe = await this.recipeRepository.GetRecipeById(recipeId);
            return new GetRecipeWithStepsAndIngredientsResponse(recipe);
        }

        private static Recipe BuildRecipeFromRequest(CreateRecipeRequest request, Dictionary<Guid, Product> products,
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
                    stepRequest.EstimatedTime)
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

        public Task<IList<Recipe>> SearchRecipeByIngredients(IList<Guid> guids)
        {
            return this.recipeRepository.FindRecipesWithIngredients(guids);
        }
    }
}