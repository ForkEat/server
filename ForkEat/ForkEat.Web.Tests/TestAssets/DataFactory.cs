using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var unit = new Unit() { Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg" };

            await this.context.Products.AddRangeAsync(product1, product2);
            await this.context.Units.AddAsync(unit);

            var recipeEntity1 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 1",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>()
                {
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product1,Unit = unit, Quantity = 1 },
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product2,Unit = unit, Quantity = 2 },
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
                },ImageId = Guid.NewGuid()
            };

            var recipeEntity2 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 2",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>()
                {
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product1,Unit = unit, Quantity = 1 },
                },
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
                },ImageId = Guid.NewGuid()
            };
            await this.context.Recipes.AddRangeAsync(recipeEntity1, recipeEntity2);
            await this.context.SaveChangesAsync();

            return (recipeEntity1, recipeEntity2);
        }
    }
}