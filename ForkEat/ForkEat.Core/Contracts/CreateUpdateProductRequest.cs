using System;

namespace ForkEat.Core.Contracts
{
    public class CreateUpdateProductRequest
    {
        public string Name { get; set; }
        public Guid ImageId { get; set; }
    }
}