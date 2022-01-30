using System;
using System.Collections.Generic;
using System.Linq;

namespace ForkEat.Core.Domain;

public class Kitchen : IKitchen
{

    public void CookRecipeFromStock(Recipe recipe,List<Stock> stocks)
    {
        foreach (Ingredient ingredient in recipe.Ingredients)
        {
            Stock stockForCurrentIngredient = stocks.First(stock => stock.Product.Id == ingredient.Product.Id);

            if (stockForCurrentIngredient.Unit.Id != ingredient.Unit.Id)
            {
                throw new ArgumentException($"Different Units for Ingredient : {ingredient}");
            }
                
            stockForCurrentIngredient.Quantity -= ingredient.Quantity;
        }
    }
}