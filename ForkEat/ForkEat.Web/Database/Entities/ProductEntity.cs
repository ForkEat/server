using System;
using ForkEat.Core.Domain;

namespace ForkEat.Web.Database.Entities;

public class ProductEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ImageId { get; set; }

    public ProductEntity()
    {
            
    }

    public ProductEntity(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        ImageId = product.ImageId;
    }
}