using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;

namespace ForkEat.Web.Database
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext dbContext;

        public RecipeRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<Recipe> InsertRecipe(Recipe recipe)
        {
            throw new System.NotImplementedException();
        }
    }
}