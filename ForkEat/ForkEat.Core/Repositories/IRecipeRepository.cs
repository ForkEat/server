using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe> InsertRecipe(Recipe recipe);
        Task<List<Recipe>> GetAllRecipes();
    }
}