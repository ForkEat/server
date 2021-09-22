using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories
{
    public interface IProductRepository
    {
        Task<Product> InsertProduct(Product product);
        Task<Product> FindProductById(Guid id);
        Task<List<Product>> FindAllProducts();
        Task DeleteProduct(Product product);
        Task<Product> UpdateProduct(Product newProduct);
    }
}