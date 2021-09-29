using System;

namespace ForkEat.Core.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
    }
}