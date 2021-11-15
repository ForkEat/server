using System;

namespace ForkEat.Core.Domain
{
    public class Stock
    {
        public Guid Id { get; }
        private double quantity;

        public double Quantity
        {
            get => quantity;
            set => quantity = value > 0 ? value : throw new ArgumentException("Stock quantity should be positive");
        }

        private Unit unit;
        public Unit Unit
        {
            get => unit;
            set => unit = value ?? throw new ArgumentException("Stock Unit should not be null");
        }

        private Product product;
        public Product Product { get => product; set => product = value ?? throw new ArgumentException("Stock Product should not be null"); }
        public DateOnly BestBeforeDate { get; set; }
        public DateOnly PurchaseDate { get; set; }

        public Stock(Guid id, double quantity, Unit unit, Product product)
        {
            Id = id != Guid.Empty ? id : throw new ArgumentException("Stock Id should not be empty");
            Quantity = quantity;
            Unit = unit;
            Product = product;
        }

        public Stock(double quantity, Unit unit, Product product)
        {
            Id = Guid.NewGuid();
            Quantity = quantity;
            Unit = unit;
            Product = product;
        }
    }
}