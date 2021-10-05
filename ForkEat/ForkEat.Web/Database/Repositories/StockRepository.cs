using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database
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
            await dbContext.Stocks.AddAsync(stock);
            await dbContext.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock> UpdateStock(Stock stock)
        {
            dbContext.Stocks.Update(stock);
            await dbContext.SaveChangesAsync();
            return await FindStockById(stock.Id);
        }

        public Task<Stock> FindStockById(Guid id)
        {
            return dbContext
                .Stocks
                .FirstOrDefaultAsync(stock => stock.Id == id);
        }

        public async Task DeleteStock(Stock stock)
        {
            dbContext.Stocks.Remove(stock);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Stock>> FindAllStocksByProductId(Guid productId)
        {
            return dbContext
                .Stocks
                .Where(stock => stock.Product.Id == productId || stock.ProductId == productId)
                .Include(stock => stock.Unit)
                .Include(stock => stock.Product);
        }
    }
}