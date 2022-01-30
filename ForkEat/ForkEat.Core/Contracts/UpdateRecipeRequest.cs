using System;

namespace ForkEat.Core.Contracts;

public class UpdateRecipeRequest : CreateRecipeRequest
{
    public Guid Id { get; set; }
        
}