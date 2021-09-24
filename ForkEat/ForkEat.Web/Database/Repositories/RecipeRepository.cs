using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Web.Database.Entities;

namespace ForkEat.Web.Database
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext dbContext;

        public RecipeRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Recipe> InsertRecipe(Recipe recipe)
        {
            var entity = new RecipeEntity()
            {
                Id = recipe.Id,
                Difficulty = recipe.Difficulty,
                Name = recipe.Name,
                Ingredients = recipe.Ingredients.Select(ingredient => new IngredientEntity()
                {
                    Product = ingredient.Product,
                    Quantity = ingredient.Quantity
                }).ToList(),
                Steps = recipe.Steps.Select(step => new StepEntity(){Id = step.Id, Name = step.Name, Instructions = step.Instructions, EstimatedTime = step.EstimatedTime}).ToList()
            };

            await this.dbContext.Recipes.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            return recipe;
        }
    }
}