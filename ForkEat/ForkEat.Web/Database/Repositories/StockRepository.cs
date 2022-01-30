using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories;

public class StockRepository : IStockRepository
{
    private readonly ApplicationDbContext dbContext;

    public StockRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Stock> InsertStock(Stock stock)
    {
        ProductEntity productEntity = await dbContext.Products.FirstAsync(p => p.Id == stock.Product.Id);

        await dbContext.Stocks.AddAsync(new StockEntity(stock) {Product = productEntity});
        await dbContext.SaveChangesAsync();
        return stock;
    }

    public async Task<Stock> UpdateStock(Stock stock)
    {
        StockEntity stockEntity = await dbContext.Stocks.FirstAsync(entity => entity.Id == stock.Id);

        stockEntity.Quantity = stock.Quantity;
        stockEntity.Unit = stock.Unit;
        stockEntity.BestBeforeDate = stock.BestBeforeDate;
        stockEntity.PurchaseDate = stock.PurchaseDate;

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
        return new Stock(
            entity.Id,
            entity.Quantity,
            entity.Unit,
            new Product(
                entity.Product.Id,
                entity.Product.Name,
                entity.Product.ImageId)
        )
        {
            PurchaseDate = entity.PurchaseDate,
            BestBeforeDate = entity.BestBeforeDate
        };
    }

    public async Task DeleteStock(Stock stock)
    {
        StockEntity stockEntity = await dbContext.Stocks.FirstAsync(entity => entity.Id == stock.Id);
        dbContext.Stocks.Remove(stockEntity);
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

    public async Task<Stock> FindStockByProductId(Guid productId)
    {
        var entity = await dbContext
            .Stocks
            .Include(stock => stock.Unit)
            .Include(stock => stock.Product)
            .Where(stock => stock.Product.Id == productId)
            .FirstOrDefaultAsync();

        return entity is null ? null : CreateStockFromStockEntity(entity);
    }

    public Task<List<Stock>> FindAllStocks()
    {
        return this.dbContext
            .Stocks
            .Include(stock => stock.Product)
            .Include(stock => stock.Unit)
            .Select(entity => CreateStockFromStockEntity(entity))
            .ToListAsync();
    }

    public Task<List<Stock>> FindAllStocksByProductIds(List<Guid> productIds)
    {
        return dbContext
            .Stocks
            .Include(stock => stock.Product)
            .Include(stock => stock.Unit)
            .Where(stock => productIds.Contains(stock.Product.Id))
            .Select(entity => CreateStockFromStockEntity(entity))
            .ToListAsync();
    }
}