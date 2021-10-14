using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IProductService
    {
        Task<GetProductResponse> CreateProduct(CreateUpdateProductRequest createUpdateProductRequest);
        Task<GetProductResponse> GetProductById(Guid id);
        Task<IList<GetProductResponse>> GetAllProducts();
        Task DeleteProduct(Guid id);
        Task<GetProductResponse> UpdateProduct(Guid id, CreateUpdateProductRequest updatedProduct);
    }
}