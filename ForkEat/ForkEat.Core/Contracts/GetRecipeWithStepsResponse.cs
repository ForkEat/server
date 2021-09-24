using System;
using System.Collections.Generic;
using System.Linq;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class GetRecipeWithStepsResponse
    {
        public GetRecipeWithStepsResponse(Recipe recipe)
        {
            Id = recipe.Id;
            Name = recipe.Name;
            Difficulty = recipe.Difficulty;
            TotalEstimatedTime = recipe.TotalEstimatedTime;
            Steps = recipe.Steps.Select(step => new GetStepResponse(step)).ToList();
            Ingredients = recipe.Ingredients.Select(i => new GetIngredientResponse(i)).ToList();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint Difficulty { get; set; }
        public TimeSpan TotalEstimatedTime { get; set; }
        public List<GetStepResponse> Steps { get; set; }
        public List<GetIngredientResponse> Ingredients { get; set; }
    }
}