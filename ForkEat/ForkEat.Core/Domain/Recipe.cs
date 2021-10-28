using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkEat.Core.Domain
{
    public class Recipe
    {
        public Guid Id { get; set; }
        private string name;
        public Guid ImageId;
        public bool IsLiked { get; set; }

        public string Name
        {
            get => name;
            set => name = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException("Invalid recipe name (null or empty)");
        }

        public List<Step> Steps { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        public TimeSpan TotalEstimatedTime => Steps
            .Select(step => step.EstimatedTime)
            .Aggregate(TimeSpan.Zero, (item, acc) => item + acc);

        private uint difficulty;

        public uint Difficulty
        {
            get => difficulty;
            set => difficulty = value <= 5 ? value : throw new ArgumentException("Recipe difficulty should be between 0 and 5");
        }

        public Recipe()
        {
        }

        public Recipe(Guid id, string name,uint difficulty, IList<Step> steps, List<Ingredient> ingredients, Guid imageId)
        {
            this.Id = id;
            this.Name = name;
            this.Steps = steps.ToList();
            this.difficulty = difficulty;
            Ingredients = ingredients.ToList();
            this.ImageId = imageId;
        }

        
    }
}