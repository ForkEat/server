using System;

namespace ForkEat.Core.Domain
{
    public class Ingredient
    {
        private Product product;

        public Product Product
        {
            get => product;
            set => product = value ?? throw new ArgumentException("Ingredient Product should not be null");
        }

        private Unit unit;
        
        public Unit Unit {
            get => unit;
            set => unit = value ?? throw new ArgumentException("Ingredient Unit should not be null");
        }
        private uint quantity;

        public uint Quantity
        {
            get => quantity;
            set => quantity = value > 0 ? value : throw new ArgumentException("Ingredient Quantity should be positive");
        }

        public Ingredient(uint quantity, Product product, Unit unit)
        {
            Quantity = quantity;
            Product = product;
            Unit = unit;
        }
    }
}