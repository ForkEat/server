using System.Collections.Generic;

namespace ForkEat.Core.Domain;

public interface IKitchen
{
    void CookRecipeFromStock(Recipe recipe, List<Stock> stocks);
}