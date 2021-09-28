using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository stockRepository;

        private readonly IProductRepository productRepository;

        private readonly IUnitRepository unitRepository;

        public StockService(IStockRepository stockRepository, IProductRepository productRepository, IUnitRepository unitRepository)
        {
            this.stockRepository = stockRepository;
            this.productRepository = productRepository;
            this.unitRepository = unitRepository;
        }

        public async Task<StockResponse> CreateOrUpdateStock(Guid productId, CreateUpdateStockRequest stockRequest)
        {
            var product = await productRepository.FindProductById(productId) ?? throw new ProductNotFoundException();
            var unit = await unitRepository.FindUnitById(stockRequest.UnitId) ?? throw new UnitNotFoundException();
            var stock = stockRequest.Id != Guid.Empty
                ? await stockRepository.FindStockById(stockRequest.Id)
                : new Stock()
                {
                    Id = Guid.NewGuid(),
                    Quantity = stockRequest.Quantity
                };

            Stock response = null;

            if (stock.Id != stockRequest.Id)
            {
                stock.Product = product;
                stock.Unit = unit;
                response =  await stockRepository.InsertStock(stock);
            }
            else
            {
                if (stockRequest.Quantity == 0)
                {
                    await stockRepository.DeleteStock(stock);
                    await stockRepository.FindAllStocksByProductId(productId);
                    return new StockResponse();
                }

                stock.Quantity = stockRequest.Quantity;
                response = await stockRepository.UpdateStock(stock);
            }

            return new StockResponse()
            {
                Id = response.Id,
                Quantity = response.Quantity,
                Unit = new UnitResponse()
                {
                    Name = response.Unit.Name,
                    Symbol = response.Unit.Symbol,
                }
            };
        }

        public async Task<IEnumerable<StockResponse>> GetStocks(Guid productId)
        {
            IList<StockResponse> response = new List<StockResponse>();
            var result = (await stockRepository.FindAllStocksByProductId(productId)).ToList();
            result.ForEach(stock => response.Add(new StockResponse()
            {
                Id = stock.Id,
                Quantity = stock.Quantity,
                Unit = new UnitResponse()
                {
                    Name = stock.Unit.Name,
                    Symbol = stock.Unit.Symbol
                }
            }));

            return response;
        }
    }
}