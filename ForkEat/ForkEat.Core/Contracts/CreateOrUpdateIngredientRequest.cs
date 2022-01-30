using System;

namespace ForkEat.Core.Contracts;

public class CreateOrUpdateIngredientRequest
{
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
    public Guid UnitId { get; set; }
}