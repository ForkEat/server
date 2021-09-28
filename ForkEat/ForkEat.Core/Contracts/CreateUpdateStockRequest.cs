using System;

namespace ForkEat.Core.Contracts
{
    public class CreateUpdateStockRequest
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public Guid UnitId { get; set; }
    }
}