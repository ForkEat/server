using System;

namespace ForkEat.Core.Contracts
{
    public class CreateOrUpdateStepRequest
    {
        public string Name { get; set; }
        public string Instructions { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}