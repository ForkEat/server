using System;
using System.Collections.Generic;

namespace ForkEat.Core.Contracts
{
    public class GetRecipeWithStepsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint Difficulty { get; set; }
        public TimeSpan TotalEstimatedTime { get; set; }
        public List<GetStepResponse> Steps { get; set; }
    }
}