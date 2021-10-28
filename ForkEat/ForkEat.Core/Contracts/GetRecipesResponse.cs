using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class GetRecipesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
        public uint Difficulty { get; set; }
        public TimeSpan TotalEstimatedTime { get; set; }
        public bool IsLiked { get; set; }

        public GetRecipesResponse() {}
        
        public GetRecipesResponse(Recipe recipe)
        {
            Id = recipe.Id;
            Name = recipe.Name;
            ImageId = recipe.ImageId;
            Difficulty = recipe.Difficulty;
            TotalEstimatedTime = recipe.TotalEstimatedTime;
        }
    }
}