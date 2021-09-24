using System;
using System.Collections.Generic;

namespace ForkEat.Web.Database.Entities
{
    public class RecipeEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint Difficulty { get; set; }
        public TimeSpan TotalEstimatedTime { get; set; }
        public List<StepEntity> Steps { get; set; }
        public List<IngredientEntity> Ingredients { get; set; }
    }
}