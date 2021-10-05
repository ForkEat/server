using System;
using System.Collections.Generic;

namespace ForkEat.Core.Contracts
{
    public class CreateRecipeRequest
    {


        public string Name { get; set; }
        public Guid ImageId { get; set; }
        public uint Difficulty { get; set; }
        public List<CreateOrUpdateStepRequest> Steps { get; set; }
        public List<CreateOrUpdateIngredientRequest> Ingredients { get; set; }
    }
}