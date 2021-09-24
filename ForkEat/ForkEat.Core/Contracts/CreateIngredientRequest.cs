using System;

namespace ForkEat.Core.Contracts
{
    public class CreateIngredientRequest
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}