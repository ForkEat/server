using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IProductService
    {
        Task<Product> CreateProduct(CreateProductRequest createProductRequest);
        Task<Product> GetProductById(Guid id);
        Task<IList<Product>> GetAllProducts();
    }
}