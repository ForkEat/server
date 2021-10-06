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

        public (Product, Product) CreateProducts()
        {
            var product1 = new Product(Guid.NewGuid(), "Product 1", Guid.NewGuid());
            var product2 = new Product(Guid.NewGuid(), "Product 2", Guid.NewGuid());
            return (product1, product2);
        }
        
        public async Task<(ProductEntity, ProductEntity)> CreateAndInsertProducts()
        {
            var product1 = new ProductEntity(){ Id = Guid.NewGuid(),Name =  "Product 1", ImageId = Guid.NewGuid()};
            var product2 = new ProductEntity(){ Id = Guid.NewGuid(),Name =  "Product 2", ImageId = Guid.NewGuid()};

            await context.AddRangeAsync(product1, product2);
            await context.SaveChangesAsync();
            
            return (product1, product2);
        }

        public async Task<ProductEntity> CreateAndInsertProduct(Guid id, string name, Guid imageId)
        {
            var product1 = new ProductEntity(){ Id = id,Name = name, ImageId =imageId};

            await context.AddAsync(product1);
            await context.SaveChangesAsync();
            
            return product1;
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
        
        public Product CreateCarrotProduct()
        {
            var productName = "carrot";
            var productId = Guid.NewGuid();

            return new Product
            (
                productId, productName + " " + productId,
                Guid.NewGuid()
            );
        }

        public async Task<(RecipeEntity, RecipeEntity)> CreateAndInsertRecipesWithIngredientsAndSteps()
        {
            var (product1, product2) = await CreateAndInsertProducts();
            var unit = new Unit() { Id = Guid.NewGuid(), Name = "Kilogramme", Symbol = "kg" };
            
            await this.context.Units.AddAsync(unit);

            var recipeEntity1 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 1",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>()
                {
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product1, Unit = unit, Quantity = 1 },
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product2, Unit = unit, Quantity = 2 },
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
                },
                ImageId = Guid.NewGuid()
            };

            var recipeEntity2 = new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Test Recipe 2",
                Difficulty = 1,
                Ingredients = new List<IngredientEntity>()
                {
                    new IngredientEntity() { Id = Guid.NewGuid(), Product = product1, Unit = unit, Quantity = 1 },
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
                },
                ImageId = Guid.NewGuid()
            };
            await this.context.Recipes.AddRangeAsync(recipeEntity1, recipeEntity2);
            await this.context.SaveChangesAsync();

            return (recipeEntity1, recipeEntity2);
        }
    }
}