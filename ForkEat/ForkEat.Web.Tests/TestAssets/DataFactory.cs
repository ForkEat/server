using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Entities;

namespace ForkEat.Web.Tests.TestAssets
{
    public class DataFactory
    {
        private readonly ApplicationDbContext context;

        public DataFactory(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        public Unit CreateUnit(string name, string symbol)
        {
            var unitId = Guid.NewGuid();
            
            return new Unit()
            {
                Id = unitId,
                Name = name,
                Symbol = symbol
            };
        }
        
        public Stock CreateStock(Guid stockId, Product product, Unit unit)
        {
            return new Stock()
            {
                Id = stockId,
                Quantity = 8,
                Unit = unit,
                Product = product
            };
        }
        
        public Product CreateCarrotProduct()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();

            return new Product
            {
                Id = productId,
                Name = productName + " " + productId,
                ImageId = Guid.NewGuid()
            };
        }
        
        public async Task<(RecipeEntity, RecipeEntity)> CreateAndInsertRecipesWithIngredientsAndSteps()
        {
            var product1 = new Product() { Name = "Test Product 1" };
            var product2 = new Product() { Name = "Test Product 2" };

            await this.context.Products.AddRangeAsync(product1, product2);

            var recipeEntity1 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 1",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>()
                {
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product1, Quantity = 1 },
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product2, Quantity = 2 },
                },
                Steps = new List<StepEntity>()
                {
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 1", Instructions = "Test Step 1 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0),
                        Order = 0
                    },
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 2", Instructions = "Test Step 2 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0),
                        Order = 1
                    }
                }
            };

            var recipeEntity2 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 2",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>() { },
                Steps = new List<StepEntity>()
                {
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 3", Instructions = "Test Step 3 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    },
                    new StepEntity()
                    {
                        Id = Guid.NewGuid(), Name = "Test Step 4", Instructions = "Test Step 4 Instructions",
                        EstimatedTime = new TimeSpan(0, 1, 0)
                    }
                }
            };
            await this.context.Recipes.AddRangeAsync(recipeEntity1, recipeEntity2);
            await this.context.SaveChangesAsync();

            return (recipeEntity1, recipeEntity2);
        }
    }
}