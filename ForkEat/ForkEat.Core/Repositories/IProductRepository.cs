using System;
using System.Threading.Tasks;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories
{
    public interface IProductRepository
    {
        Task<Product> InsertProduct(Product product);
        Task<Product> FindProductById(Guid id);
    }
}