using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;

namespace ForkEat.Core.Services
{
    public interface IStockService
    {
        Task<StockResponse> CreateOrUpdateStock(Guid productId, CreateUpdateStockRequest stock);
        Task<IEnumerable<StockResponse>> GetStocks(Guid productId);
        Task<List<ProductStockResponse>> GetCompleteStock();
    }
}