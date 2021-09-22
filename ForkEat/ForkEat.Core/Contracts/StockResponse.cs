using System;

namespace ForkEat.Core.Contracts
{
    public class StockResponse
    {
        public Guid Id { get; set; }
        public float Quantity { get; set; }
        public UnitResponse Unit { get; set; }
    }
}