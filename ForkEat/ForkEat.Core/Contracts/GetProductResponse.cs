using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class GetProductResponse
{


    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ImageId { get; set; }
    public ProductType ProductType { get; set; }

    public GetProductResponse()
    {
    }

    public GetProductResponse(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        ImageId = product.ImageId;
        ProductType = product.ProductType;
    }
}