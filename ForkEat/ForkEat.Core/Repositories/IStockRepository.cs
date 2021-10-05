using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> InsertStock(Stock stock);
        Task<Stock> UpdateStock(Stock stock);
        Task<Stock> FindStockById(Guid id);
        Task DeleteStock(Stock stock);
        Task<IEnumerable<Stock>> FindAllStocksByProductId(Guid productId);
    }
}