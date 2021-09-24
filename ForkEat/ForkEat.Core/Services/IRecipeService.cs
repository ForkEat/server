using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IRecipeService
    {
        Task<GetRecipeWithStepsAndIngredientsResponse> CreateRecipe(CreateRecipeRequest request);
        Task<GetRecipesReponse> GetRecipes();
    }
}