using System;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository repository;

        public RecipeService(IRecipeRepository repository)
        {
            this.repository = repository;
        }

        public async Task<GetRecipeWithStepsResponse> CreateRecipe(CreateRecipeRequest request)
        {
            var recipe = new Recipe(
                Guid.NewGuid(),
                request.Name,
                request.Difficulty,
                request.Steps.Select(stepRequest => new Step(
                    Guid.NewGuid(),
                    stepRequest.Name,
                    stepRequest.Instructions,
                    stepRequest.EstimatedTime)
                ).ToList());

            recipe = await this.repository.InsertRecipe(recipe);

            return new GetRecipeWithStepsResponse()
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Difficulty = recipe.Difficulty,
                TotalEstimatedTime = recipe.TotalEstimatedTime,
                Steps = recipe.Steps.Select(step => new GetStepResponse()
                {
                    Id = step.Id,
                    Name = step.Name,
                    EstimatedTime = step.EstimatedTime,
                    Instructions = step.Instructions
                }).ToList()
            };
        }
    }
}