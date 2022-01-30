using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class GetStepResponse
{
    public GetStepResponse(){}
        
    public GetStepResponse(Step step)
    {
        Name = step.Name;
        EstimatedTime = step.EstimatedTime;
        Instructions = step.Instructions;
    }

    public string Name { get; set; }
    public string Instructions { get; set; }
    public TimeSpan EstimatedTime { get; set; }
}