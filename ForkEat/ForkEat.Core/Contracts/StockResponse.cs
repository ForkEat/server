using System;

namespace ForkEat.Core.Contracts
{
    public class StockResponse
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public UnitResponse Unit { get; set; }
    }
}