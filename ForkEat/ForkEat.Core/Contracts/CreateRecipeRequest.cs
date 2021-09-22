using System.Collections.Generic;

namespace ForkEat.Core.Contracts
{
    public class CreateRecipeRequest
    {
        public string Name { get; set; }
        public uint Difficulty { get; set; }
        public List<CreateStepRequest> Steps { get; set; }
    }
}