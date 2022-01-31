using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using Moq;
using Xunit;

namespace ForkEat.Core.Tests.Services;

public class ProductTypeServiceTests
{
    [Fact]
    public async Task CreateProductType_WithValidParams_ReturnsProductType()
    {
        var name = "fruit";

        var mockRepository = new Mock<IProductTypeRepository>();

        ProductType insertedProductType = null;

        mockRepository.Setup(mock => mock.InsertProductType(It.IsAny<ProductType>()))
            .Returns<ProductType>(unit =>
            {
                insertedProductType = unit;
                insertedProductType.Id = Guid.NewGuid();
                return Task.FromResult(insertedProductType);
            });

        var service = new ProductTypeService(mockRepository.Object);

        var productTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = name
        };

        var result = await service.CreateProductType(productTypeRequest);
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetProductTypeById_WithExistingProductType_ReturnsProductType()
    {
        var name = "fruit";
        var id = Guid.NewGuid();

        var mockRepository = new Mock<IProductTypeRepository>();

        mockRepository.Setup(mock => mock.FindProductTypeById(id))
            .Returns<Guid>(_ => Task.FromResult(new ProductType
            {
                Id = id,
                Name = name
            }));

        var service = new ProductTypeService(mockRepository.Object);

        var result = await service.GetProductTypeById(id);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetProductTypeById_WithNonExistingProductType_ThrowsException()
    {
        var mockRepository = new Mock<IProductTypeRepository>();

        mockRepository.Setup(mock => mock.FindProductTypeById(It.IsAny<Guid>()))
            .Returns<Guid>(_ => Task.FromResult<ProductType>(null));

        var service = new ProductTypeService(mockRepository.Object);

        await service.Invoking(unitService => unitService.GetProductTypeById(Guid.NewGuid()))
            .Should().ThrowAsync<ProductTypeNotFoundException>();
    }

    [Fact]
    public async Task GetAllProductTypes_ReturnsList()
    {
        var productType = CreateProductType("fruit");
        var productType2 = CreateProductType("vegetable");

        var productTypes = new List<ProductType>
        {
            productType,
            productType2
        };

        var mockRepository = new Mock<IProductTypeRepository>();

        mockRepository.Setup(mock => mock.FindAllProductTypes())
            .Returns(() => Task.FromResult<List<ProductType>>(productTypes));

        var service = new ProductTypeService(mockRepository.Object);

        var result = await service.GetAllProductTypes();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task DeleteProductType_WithExistingProductType_ReturnsVoid()
    {
        var name = "fruit";
        var id = Guid.NewGuid();

        var mockRepository = new Mock<IProductTypeRepository>();

        mockRepository.Setup(mock => mock.FindProductTypeById(id))
            .Returns<Guid>(_ => Task.FromResult(new ProductType
            {
                Id = id,
                Name = name
            }));

        var service = new ProductTypeService(mockRepository.Object);

        await service.Invoking(unitService => unitService.DeleteProductType(id))
            .Should().NotThrowAsync<Exception>();
    }

    [Fact]
    public async Task DeleteProductType_WithNonExistingProductType_ThrowsException()
    {
        var mockRepository = new Mock<IProductTypeRepository>();

        mockRepository.Setup(mock => mock.FindProductTypeById(It.IsAny<Guid>()))
            .Returns<Guid>(_ => Task.FromResult<ProductType>(null));

        var service = new ProductTypeService(mockRepository.Object);

        await service.Invoking(unitService => unitService.DeleteProductType(Guid.NewGuid()))
            .Should().ThrowAsync<ProductTypeNotFoundException>();
    }

    [Fact]
    public async Task UpdateProductType_WithExistingProductType_ReturnsProductType()
    {
        var name = "fruit";
        var id = Guid.NewGuid();

        var mockRepository = new Mock<IProductTypeRepository>();

        ProductType updatedProductType = null;

        mockRepository.Setup(mock => mock.FindProductTypeById(id))
            .Returns<Guid>(_ => Task.FromResult(new ProductType
            {
                Id = id,
                Name = name
            }));

        mockRepository.Setup(mock => mock.UpdateProductType(It.IsAny<ProductType>()))
            .Returns<ProductType>(productType =>
            {
                updatedProductType = productType;
                return Task.FromResult(updatedProductType);
            });

        var service = new ProductTypeService(mockRepository.Object);

        var updateProductTypeRequest = new CreateUpdateProductTypeRequest
        {
            Name = "vegetable"
        };

        var result = await service.UpdateProductType(id, updateProductTypeRequest);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be("vegetable");
    }

    [Fact]
    public async Task UpdateProductType_WithNonExistingProductType_ThrowsException()
    {
        var id = Guid.NewGuid();

        var updateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = "vegetable"
        };

        var mockRepository = new Mock<IProductTypeRepository>();

        mockRepository.Setup(mock => mock.FindProductTypeById(It.IsAny<Guid>()))
            .Returns<Guid>(_ => Task.FromResult<ProductType>(null));

        var service = new ProductTypeService(mockRepository.Object);

        await service.Invoking(unitService => unitService.UpdateProductType(id, updateProductTypeRequest))
            .Should().ThrowAsync<ProductTypeNotFoundException>();
    }

    private ProductType CreateProductType(string unitName)
    {
        var id = Guid.NewGuid();

        return new ProductType()
        {
            Id = id,
            Name = unitName
        };
    }
}