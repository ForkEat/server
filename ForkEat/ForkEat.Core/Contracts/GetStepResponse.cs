using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class GetStepResponse
    {
        public GetStepResponse(Step step)
        {
            Id = step.Id;
            Name = step.Name;
            EstimatedTime = step.EstimatedTime;
            Instructions = step.Instructions;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}