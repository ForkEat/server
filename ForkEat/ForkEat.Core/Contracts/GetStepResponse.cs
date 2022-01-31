using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class GetStepResponse
{
    public GetStepResponse(){}
        
    public GetStepResponse(Step step)
    {
        Name = step.Name;
        EstimatedTime = (uint) step.EstimatedTime.TotalSeconds;
        Instructions = step.Instructions;
    }

    public string Name { get; set; }
    public string Instructions { get; set; }
    public uint EstimatedTime { get; set; }
}