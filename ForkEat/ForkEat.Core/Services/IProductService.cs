using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IProductService
    {
        Task<Product> CreateProduct(CreateUpdateProductRequest createUpdateProductRequest);
        Task<Product> GetProductById(Guid id);
        Task<IList<Product>> GetAllProducts();
        Task DeleteProduct(Guid id);
        Task<Product> UpdateProduct(Guid id, CreateUpdateProductRequest updatedProduct);
    }
}