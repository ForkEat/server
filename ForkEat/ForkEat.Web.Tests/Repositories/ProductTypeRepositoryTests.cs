using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories;

public class ProductTypeRepositoryTests : RepositoryTest
{
    public ProductTypeRepositoryTests() : base(new []{"ProductTypes"})
    {
    }

    [Fact]
    public async Task InsertProductType_InsertRecordInDatabase()
    {
        // Given
        var name = "fruit";
        var productType = new ProductType
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        var repository = new ProductTypeRepository(context);

        // When
        var result = await repository.InsertProductType(productType);

        // Then
        context.ProductTypes.Should().ContainSingle();

        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(name);

        var productTypeInDb = await context.ProductTypes.FirstAsync(productType => productType.Id == result.Id);
        productTypeInDb.Id.Should().Be(result.Id);
        productTypeInDb.Name.Should().Be(name);
    }

    [Fact]
    public async Task FindProductTypeById_ExistingProductType_ReturnsProductType()
    {
        // Given
        var name = "fruit";
        var id = Guid.NewGuid();
        var productType = new ProductType
        {
            Id = id,
            Name = name
        };
        var repository = new ProductTypeRepository(context);

        await context.ProductTypes.AddAsync(productType);
        await context.SaveChangesAsync();

        // When
        var result = await repository.FindProductTypeById(id);

        // Then
        result.Id.Should().Be(result.Id);
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task FindProductTypeById_NonExistingProductType_ReturnsNull()
    {
        // Given
        var productTypeName = "fruit";
        var productTypeId = Guid.NewGuid();
        var productType = new ProductType
        {
            Id = productTypeId,
            Name = productTypeName
        };
        var repository = new ProductTypeRepository(context);

        await context.ProductTypes.AddAsync(productType);
        await context.SaveChangesAsync();

        // When
        var result = await repository.FindProductTypeById(Guid.NewGuid());

        // Then
        result.Should().BeNull();
    }

    [Fact]
    public async Task FindAllProductTypes_ReturnsList()
    {
        // Given
        var repository = new ProductTypeRepository(context);

        var productType = dataFactory.CreateProductType("fruit");
        var productType2 = dataFactory.CreateProductType("vegetable");

        await context.ProductTypes.AddAsync(productType);
        await context.ProductTypes.AddAsync(productType2);
        await context.SaveChangesAsync();

        // When
        var result = await repository.FindAllProductTypes();

        // Then
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteProductType_WithExistingProductType_ReturnsVoid()
    {
        // Given
        var productTypeName = "fruit";
        var productTypeId = Guid.NewGuid();
        var productType = new ProductType
        {
            Id = productTypeId,
            Name = productTypeName
        };
        var repository = new ProductTypeRepository(context);

        await context.ProductTypes.AddAsync(productType);
        await context.SaveChangesAsync();

        // Then
        await repository.Invoking(productTypeRepository => productTypeRepository.DeleteProductType(productType))
            .Should().NotThrowAsync<Exception>();
    }

    [Fact]
    public async Task UpdateProductType_WithExistingProductType_ReturnsProductType()
    {
        // Given
        var productTypeName = "fruit";
        var productTypeId = Guid.NewGuid();
        var productType = new ProductType
        {
            Id = productTypeId,
            Name = productTypeName
        };

        var repository = new ProductTypeRepository(context);

        await context.ProductTypes.AddAsync(productType);
        await context.SaveChangesAsync();

        //When
        productType.Name = "vegetable";
        var result = await repository.UpdateProductType(productType);

        // Then
        result.Id.Should().Be(productTypeId);
        result.Name.Should().Be("vegetable");
    }

    [Fact]
    public async Task FindProductsByIds_ReturnsOnlyExpectedProducts()
    {
        // Given
        var productTypes = new ProductType[]
        {
            new ProductType() { Id = Guid.NewGuid(), Name = "fruit"},
            new ProductType() { Id = Guid.NewGuid(), Name = "vegetable"},
            new ProductType() { Id = Guid.NewGuid(), Name = "pasta"},
        };
        await context.ProductTypes.AddRangeAsync(productTypes);
        await context.SaveChangesAsync();

        var repository = new ProductTypeRepository(context);

        var productTypesIds = productTypes
            .Take(2)
            .Select(product => product.Id)
            .ToList();

        // When
        var result = await repository.FindProductTypesByIds(productTypesIds);

        // Then
        result.Should().HaveCount(2);
        result.Should().ContainKeys(productTypesIds);
        result[productTypesIds[0]].Name.Should().Be("fruit");
        result[productTypesIds[1]].Name.Should().Be("vegetable");
    }
}