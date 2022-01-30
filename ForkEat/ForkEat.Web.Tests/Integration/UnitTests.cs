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

public class UnitTests : AuthenticatedTests
{
    public UnitTests(WebApplicationFactory<Startup> factory) : base(factory, new string[]{"Stocks","Units"})
    {
    }

    [Fact]
    public async Task CreateUnit_withValidParams_Returns201()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
            
        // Given
            
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };
        
        // When
        var response = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadAsAsync<Unit>();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(unitName);
        result.Symbol.Should().Be(unitSymbol);
    }
        
    [Fact]
    public async Task GetUnitById_WithExistingUnit_Returns200()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };
            
        // Given
            
        var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
        var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
        var unitId = createdUnitResult.Id;
            
        // When
        var response = await client.GetAsync("/api/units/" + unitId);
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<Unit>();
        result.Id.Should().Be(unitId);
        result.Name.Should().Be(unitName);
        result.Symbol.Should().Be(unitSymbol);
    }
        
    [Fact]
    public async Task GetUnitById_WithNonExistingUnit_Returns404()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };
            
        // Given
            
        await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            
        // When
        var response = await client.GetAsync("/api/units/" + Guid.NewGuid());
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
        
    [Fact]
    public async Task GetAllUnit_Returns200()
    {
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = "kilogram",
            Symbol = "kg"
        };
            
        var createUpdateUnitRequest2 = new CreateUpdateUnitRequest()
        {
            Name = "unit",
            Symbol = "ut"
        };
            
        // Given
            
        await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
        await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest2);
            
        // When
        var response = await client.GetAsync("/api/units");
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsAsync<IEnumerable<Unit>>();
        result.Should().HaveCount(2);
    }
        
    [Fact]
    public async Task DeleteUnit_WithExistingUnit_Returns200()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };
            
        // Given
            
        var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
        var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
        var unitId = createdUnitResult.Id;
            
        // When
        var response = await client.DeleteAsync("/api/units/" + unitId);
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponse = await client.GetAsync("/api/unit/" + unitId);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
        
    [Fact]
    public async Task DeleteUnit_WithNonExistingUnit_Returns404()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };
            
        // Given
            
        await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            
        // When
        var response = await client.DeleteAsync("/api/units/" + Guid.NewGuid());
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
        
    [Fact]
    public async Task UpdateUnit_WithExistingUnit_Returns200()
    {
        var unitName = "kilogram";
        var unitSymbol = "kg";
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = unitName,
            Symbol = unitSymbol
        };
            
        // Given
            
        var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
        var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
        var unitId = createdUnitResult.Id;
            
        // When
        var createUpdateUnitRequestUpdated = new CreateUpdateUnitRequest()
        {
            Name = unitName + " updated"
        };
        var response = await client.PutAsJsonAsync("/api/units/" + unitId, createUpdateUnitRequestUpdated);
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponse = await client.GetAsync("/api/units/" + unitId);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResult = await getResponse.Content.ReadAsAsync<Unit>();
        getResult.Id.Should().Be(unitId);
        getResult.Name.Should().Be(unitName + " updated");
        getResult.Symbol.Should().Be(unitSymbol);
    }
        
    [Fact]
    public async Task UpdateUnit_WithNonExistingUnit_Returns404()
    {
        var createUpdateUnitRequest = new CreateUpdateUnitRequest()
        {
            Name = "kilogram"
        };
            
        // Given
            
        await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            
        // When
        var createUpdateUnitRequestUpdated = new CreateUpdateUnitRequest()
        {
            Name = "kilogram updated"
        };
        var response = await client.PutAsJsonAsync("/api/units/" + Guid.NewGuid(), createUpdateUnitRequestUpdated);
            
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}