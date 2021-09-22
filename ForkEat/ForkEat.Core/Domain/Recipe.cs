using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkEat.Core.Domain
{
    public class Recipe
    {
        public Guid Id { get; set; }
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Invalid recipe name (null or empty)");
                }

                this.name = value;
            } }
        public List<Step> Steps { get; set; }

        public TimeSpan TotalEstimatedTime => Steps
            .Select(step => step.EstimatedTime)
            .Aggregate(TimeSpan.Zero, (item, acc) => item + acc);

        private uint difficulty;

        public uint Difficulty
        {
            get => difficulty;
            set
            {
                if (value > 5)
                {
                    throw new ArgumentException("Recipe difficulty should be between 0 and 5");
                }

                difficulty = value;
            }
        }
        
        public Recipe(Guid id, string name,uint difficulty, IList<Step> steps)
        {
            this.Id = id;
            this.Name = name;
            this.Steps = steps.ToList();
            this.difficulty = difficulty;
        }

        
    }
}