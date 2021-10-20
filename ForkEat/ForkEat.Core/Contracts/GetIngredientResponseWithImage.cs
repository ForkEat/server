using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class GetIngredientResponseWithImage
    {
        public GetIngredientResponseWithImage()
        {
        }

        public GetIngredientResponseWithImage(Ingredient ingredient)
        {
            ImageId = ingredient.Product.ImageId;
            Name = ingredient.Product.Name;
            ProductId = ingredient.Product.Id;
            Quantity = ingredient.Quantity;
            Unit = new UnitResponse(ingredient.Unit);
        }

        public Guid ImageId { get; set; }
        public string Name { get; set; }
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
        public UnitResponse Unit { get; set; }
    }
}