using System;

namespace ForkEat.Web.Database.Entities
{
    public class StepEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}