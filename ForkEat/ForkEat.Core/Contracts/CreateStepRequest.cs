using System;

namespace ForkEat.Core.Contracts
{
    public class CreateStepRequest
    {
        public string Name { get; set; }
        public string Instructions { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}