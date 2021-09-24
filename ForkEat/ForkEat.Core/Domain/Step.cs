using System;

namespace ForkEat.Core.Domain
{
    public class Step
    {
        public Guid Id { get; }
        private string name;

        public string Name
        {
            get => name;
            set => name = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException("Step Name should not be null nor empty");
        }

        private string instructions;

        public string Instructions
        {
            get => instructions;
            set => instructions = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException("Step Instructions should not be null nor empty");
        }

        private TimeSpan estimatedTime;

        public TimeSpan EstimatedTime
        {
            get => estimatedTime;
            set => estimatedTime = value != TimeSpan.Zero
                ? value
                : throw new ArgumentException("Step EstimatedTime should not be 0");
        }

        public Step(Guid id, string name, string instructions, TimeSpan estimatedTime)
        {
            this.Id = id;
            this.Name = name;
            this.Instructions = instructions;
            this.EstimatedTime = estimatedTime;
        }
    }
}