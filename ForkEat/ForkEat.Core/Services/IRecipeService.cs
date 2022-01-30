using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;

namespace ForkEat.Core.Services;

public interface IRecipeService
{
    Task PerformRecipe(Guid recipeId);
    Task<GetRecipeWithStepsAndIngredientsResponse> CreateRecipe(CreateRecipeRequest request);
    Task<IList<GetRecipesResponse>> GetRecipes(Guid userId);
    Task<GetRecipeWithStepsAndIngredientsResponse> GetRecipeById(Guid recipeId, Guid userId);
    Task DeleteRecipeById(Guid recipeId);
    Task<GetRecipeWithStepsAndIngredientsResponse> UpdateRecipe(Guid recipeId, UpdateRecipeRequest request);
    Task<IList<GetRecipesResponse>> SearchRecipeByIngredients(IList<Guid> guids);
    Task<bool> LikeRecipe(Guid userId, Guid recipeId);
    Task UnlikeRecipe(Guid userId, Guid recipeId);
}