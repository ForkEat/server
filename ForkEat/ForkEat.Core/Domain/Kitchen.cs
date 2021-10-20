using System.Collections.Generic;

namespace ForkEat.Core.Domain
{
    public class Kitchen : IKitchen
    {
        private readonly List<Stock> stock;

        public Kitchen(List<Stock> stock)
        {
            this.stock = stock;
        }

        public void CookRecipe(Recipe recipe)
        {
            throw new System.NotImplementedException();
        }
    }
}