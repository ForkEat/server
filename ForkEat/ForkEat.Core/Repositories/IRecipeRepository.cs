using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe> InsertRecipe(Recipe recipe);
        Task<List<Recipe>> GetAllRecipes();
        Task DeleteRecipeById(Guid recipeId);
        Task<Recipe> GetRecipeById(Guid recipe1Id);
        Task<IList<Recipe>> FindRecipesWithIngredients(IList<Guid> isAny);
    }
}