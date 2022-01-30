using System;
using System.Collections.Generic;
using System.Linq;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class GetRecipeWithStepsAndIngredientsResponse
{
    public GetRecipeWithStepsAndIngredientsResponse()
    {
    }

    public GetRecipeWithStepsAndIngredientsResponse(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        ImageId = recipe.ImageId;
        Difficulty = recipe.Difficulty;
        TotalEstimatedTime = recipe.TotalEstimatedTime;
        Steps = recipe.Steps.Select(step => new GetStepResponse(step)).ToList();
        Ingredients = recipe.Ingredients.Select(i => new GetIngredientResponseWithImage(i)).ToList();
        IsLiked = recipe.IsLiked;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ImageId { get; set; }
    public uint Difficulty { get; set; }
    public TimeSpan TotalEstimatedTime { get; set; }
    public List<GetStepResponse> Steps { get; set; }
    public List<GetIngredientResponseWithImage> Ingredients { get; set; }
    public bool IsLiked { get; set; }
}