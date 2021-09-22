using System;

namespace ForkEat.Core.Contracts
{
    public class Stock
    {
        public Guid Id { get; set; }
        public float Quantity { get; set; }
        public Unit Unit { get; set; }
    }
}