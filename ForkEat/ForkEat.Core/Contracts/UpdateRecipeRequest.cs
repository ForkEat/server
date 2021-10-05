using System;
using System.Collections.Generic;

namespace ForkEat.Core.Contracts
{
    public class UpdateRecipeRequest : CreateRecipeRequest
    {
        public Guid Id { get; set; }
        
    }
}