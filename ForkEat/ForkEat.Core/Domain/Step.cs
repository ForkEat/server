using System;

namespace ForkEat.Core.Domain
{
    public class Step
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public TimeSpan EstimatedTime { get; set; }

        public Step(Guid id, string name, string instructions, TimeSpan estimatedTime)
        {
            this.Id = id;
            this.Name = name;
            this.Instructions = instructions;
            this.EstimatedTime = estimatedTime;
        }
    }
}