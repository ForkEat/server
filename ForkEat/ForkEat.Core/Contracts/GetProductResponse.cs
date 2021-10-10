using System;

namespace ForkEat.Core.Contracts
{
    public class GetProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
    }
}