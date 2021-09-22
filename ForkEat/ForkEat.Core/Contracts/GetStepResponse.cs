using System;

namespace ForkEat.Core.Contracts
{
    public class GetStepResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}