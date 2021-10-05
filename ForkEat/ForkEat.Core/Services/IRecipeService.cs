using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IRecipeService
    {
        Task<GetRecipeWithStepsAndIngredientsResponse> CreateRecipe(CreateRecipeRequest request);
        Task<List<GetRecipesResponse>> GetRecipes();
        Task<GetRecipeWithStepsAndIngredientsResponse> GetRecipeById(Guid recipeId);
        Task DeleteRecipeById(Guid recipeId);
        Task<GetRecipeWithStepsAndIngredientsResponse> UpdateRecipe(Guid recipeId, UpdateRecipeRequest request);
    }
}