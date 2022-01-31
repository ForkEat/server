using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ForkEat.Web.Tests.Integration;

public class ProductTypeTests : AuthenticatedTests
{
    public ProductTypeTests(WebApplicationFactory<Startup> factory) : base(factory, new []{"ProductTypes"})
    {
    }

    [Fact]
    public async Task CreateProductType_withValidParams_Returns201()
    {
        var name = "vegetable";

        // Given
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest
        {
            Name = name
        };

        // When
        var response = await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadAsAsync<ProductType>();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetProductTypeById_WithExistingProductType_Returns200()
    {
        var name = "fruit";
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = name
        };

        // Given
        var createdResponse = await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);
        var createdResult = await createdResponse.Content.ReadAsAsync<ProductType>();
        var productTypeId = createdResult.Id;

        // When
        var response = await client.GetAsync("/api/product-types/" + productTypeId);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<ProductType>();
        result.Id.Should().Be(productTypeId);
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetProductTypeById_WithNonExistingProductType_Returns404()
    {
        var name = "vegetable";

        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = name,
        };

        // Given

        await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);

        // When
        var response = await client.GetAsync("/api/product-types/" + Guid.NewGuid());

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllProductTypes_Returns200()
    {
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = "vegetable"
        };

        var createUpdateProductTypeRequest2 = new CreateUpdateProductTypeRequest()
        {
            Name = "fruit"
        };

        // Given
        await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);
        await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest2);

        // When
        var response = await client.GetAsync("/api/product-types");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<IEnumerable<ProductType>>();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteProductType_WithExistingProductType_Returns200()
    {
        var name = "vegetable";
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest
        {
            Name = name
        };

        // Given
        var createdResponse = await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);
        var createdResult = await createdResponse.Content.ReadAsAsync<ProductType>();
        var productTypeId = createdResult.Id;

        // When
        var response = await client.DeleteAsync("/api/product-types/" + productTypeId);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponse = await client.GetAsync("/api/product-types/" + productTypeId);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProductType_WithNonExistingProductType_Returns404()
    {
        var name = "vegetable";
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = name,
        };

        // Given
        await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);

        // When
        var response = await client.DeleteAsync("/api/product-types/" + Guid.NewGuid());

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProductType_WithExistingProductType_Returns200()
    {
        var name = "vegetable";
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = name
        };

        // Given

        var createdResponse = await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);
        var createdResult = await createdResponse.Content.ReadAsAsync<ProductType>();
        var productTypeId = createdResult.Id;

        // When
        var createUpdateProductTypeRequestUpdated = new CreateUpdateProductTypeRequest()
        {
            Name = name + " updated"
        };
        var response = await client.PutAsJsonAsync("/api/product-types/" + productTypeId, createUpdateProductTypeRequestUpdated);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponse = await client.GetAsync("/api/product-types/" + productTypeId);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResult = await getResponse.Content.ReadAsAsync<ProductType>();
        getResult.Id.Should().Be(productTypeId);
        getResult.Name.Should().Be(name + " updated");
    }

    [Fact]
    public async Task UpdateProductType_WithNonExistingProductType_Returns404()
    {
        var createUpdateProductTypeRequest = new CreateUpdateProductTypeRequest()
        {
            Name = "fruit"
        };

        // Given

        await client.PostAsJsonAsync("/api/product-types", createUpdateProductTypeRequest);

        // When
        var updateRequest = new CreateUpdateProductTypeRequest
        {
            Name = "vegetable"
        };
        var response = await client.PutAsJsonAsync("/api/product-types/" + Guid.NewGuid(), updateRequest);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}