using System;

namespace ForkEat.Core.Contracts
{
    public class CreateUpdateStockRequest
    {
        public double Quantity { get; set; }
        public Guid UnitId { get; set; }
    }
}