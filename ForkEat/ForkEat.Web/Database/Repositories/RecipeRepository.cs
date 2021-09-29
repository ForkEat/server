using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories
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
                Steps = recipe.Steps.Select((step, index) => new StepEntity()
                {
                    Id = step.Id, Name = step.Name, Instructions = step.Instructions, EstimatedTime = step.EstimatedTime, Order = (uint) index
                }).ToList()
            };

            await this.dbContext.Recipes.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            return recipe;
        }

        public Task<List<Recipe>> GetAllRecipes()
        {
            return this.dbContext.Recipes
                .Include(entity => entity.Steps)
                .OrderBy(entity => entity.Name)
                .Select(entity => CreateRecipeFromEntityWithoutIngredient(entity))
                .ToListAsync();
        }

        public async Task DeleteRecipeById(Guid recipeId)
        {
            var recipeEntity = await GetRecipeEntityById(recipeId);
            this.dbContext.Recipes.Remove(recipeEntity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<Recipe> GetRecipeById(Guid recipeId)
        {
            var recipeEntity = await GetRecipeEntityById(recipeId);
            return CreateRecipeFromEntity(recipeEntity);
        }

        
        private async Task<RecipeEntity> GetRecipeEntityById(Guid recipeId)
        {
            RecipeEntity recipeEntity = await this.dbContext
                .Recipes
                .Include(entity => entity.Ingredients).ThenInclude(ingredientEntity => ingredientEntity.Product)
                .Include(entity => entity.Steps.OrderBy(stepEntity => stepEntity.Order))
                .FirstAsync(entity => entity.Id == recipeId);
            return recipeEntity;
        }

        private static Recipe CreateRecipeFromEntity(RecipeEntity entity)
        {
            return new Recipe(
                entity.Id, 
                entity.Name, 
                entity.Difficulty,
                entity.Steps.Select(stepEntity => 
                    new Step(
                        stepEntity.Id,
                        stepEntity.Name,
                        stepEntity.Instructions,
                        stepEntity.EstimatedTime
                        )
                ).ToList(),
                entity.Ingredients.Select(ingredientEntity =>
                    new Ingredient(
                        ingredientEntity.Product,
                        ingredientEntity.Quantity
                        )
                ).ToList());
        }

        private static Recipe CreateRecipeFromEntityWithoutIngredient(RecipeEntity entity)
        {
            return new Recipe(
                entity.Id, 
                entity.Name, 
                entity.Difficulty,
                entity.Steps.Select(stepEntity => 
                    new Step(
                        stepEntity.Id,
                        stepEntity.Name, 
                        stepEntity.Instructions,
                        stepEntity.EstimatedTime
                        )
                ).ToList(), new List<Ingredient>());
        }
    }
}