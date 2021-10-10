using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StockRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Stock> InsertStock(Stock stock)
        {
            await dbContext.Stocks.AddAsync(new StockEntity(stock));
            await dbContext.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock> UpdateStock(Stock stock)
        {
            var productEntity = await dbContext.Products.FirstAsync(p => p.Id == stock.Product.Id);
            var stockEntity = new StockEntity(stock) {Product = productEntity};
            dbContext.Stocks.Update(stockEntity);
            await dbContext.SaveChangesAsync();
            return await FindStockById(stock.Id);
        }

        public async Task<Stock> FindStockById(Guid id)
        {
            var entity = await dbContext
                .Stocks
                .Include(stock => stock.Product)
                .Include(stock => stock.Unit)
                .FirstOrDefaultAsync(stock => stock.Id == id);

            return CreateStockFromStockEntity(entity);
        }

        private static Stock CreateStockFromStockEntity(StockEntity entity)
        {
            return new Stock(entity.Id, entity.Quantity, entity.Unit,
                new Product(entity.Product.Id, entity.Product.Name, entity.Product.ImageId));
        }

        public async Task DeleteStock(Stock stock)
        {
            dbContext.Stocks.Remove(new StockEntity(stock));
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Stock>> FindAllStocksByProductId(Guid productId)
        {
            return dbContext
                .Stocks
                .Where(stock => stock.Product.Id == productId || stock.Product.Id == productId)
                .Include(stock => stock.Unit)
                .Include(stock => stock.Product)
                .Select(entity => CreateStockFromStockEntity(entity));
        }
    }
}