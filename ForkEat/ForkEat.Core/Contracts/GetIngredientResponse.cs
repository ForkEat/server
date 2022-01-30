using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class GetIngredientResponse
{
    public GetIngredientResponse()
    {
    }

    public GetIngredientResponse(Ingredient ingredient)
    {
        Name = ingredient.Product.Name;
        ProductId = ingredient.Product.Id;
        Quantity = ingredient.Quantity;
        Unit = new UnitResponse(ingredient.Unit);
    }

    public string Name { get; set; }
    public Guid ProductId { get; set; }
    public double Quantity { get; set; }
    public UnitResponse Unit { get; set; }
}