using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Domain
{
    public class Stock
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}