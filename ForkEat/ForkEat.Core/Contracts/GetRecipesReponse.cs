using System;

namespace ForkEat.Core.Contracts
{
    public class GetRecipesReponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint Difficulty { get; set; }
        public TimeSpan TotalEstimatedTime { get; set; }
    }
}