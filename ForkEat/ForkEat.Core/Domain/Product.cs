using System;

namespace ForkEat.Core.Domain;

public class Product
{
    public Guid Id { get; }

    private string name;

    public ProductType? ProductType { get; set; }

    public string Name
    {
        get => name;
        set => name = !string.IsNullOrEmpty(value)
            ? value
            : throw new ArgumentException("Product Name should not be null nor empty");
    }

    private Guid imageId;

    public Guid ImageId
    {
        get => imageId;
        set => imageId = value != Guid.Empty ? value : throw new ArgumentException("Product Image Id should not be empty");
    }

    public Product(Guid id, string name, Guid imageId, ProductType productType)
    {
        Id = id != Guid.Empty ? id : throw new ArgumentException("Product Id should not be empty") ;
        Name = name;
        ImageId = imageId;
        ProductType = productType;
    }

    public Product(string name, Guid imageId, ProductType productType) : this(Guid.NewGuid(), name, imageId, productType)
    {
    }
}