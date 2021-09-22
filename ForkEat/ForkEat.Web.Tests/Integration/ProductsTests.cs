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

namespace ForkEat.Web.Tests
{
    public class ProductsTests : IntegrationTest
    {
        public ProductsTests(WebApplicationFactory<Startup> factory) : base(factory, "Products")
        {
        }

        [Fact]
        public async Task CreateProduct_withValidParams_Returns201()
        {
            var productName = "carrot";
            
            // Given
            var client = factory.CreateClient();
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName
            };

            // When
            var response = await client.PostAsJsonAsync("/api/products", createProductRequest);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadAsAsync<Product>();
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);
        }
        
        [Fact]
        public async Task GetProductById_WithExistingProduct_Returns200()
        {
            var productName = "carrot";
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName
            };
            
            // Given
            var client = factory.CreateClient();
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<Product>();
            var productId = createdProductResult.Id;
            
            // When
            var response = await client.GetAsync("/api/products/" + productId);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<Product>();
            result.Id.Should().Be(productId);
            result.Name.Should().Be(productName);
        }
        
        [Fact]
        public async Task GetProductById_WithNonExistingProduct_Returns404()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot"
            };
            
            // Given
            var client = factory.CreateClient();
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var response = await client.GetAsync("/api/products/" + Guid.NewGuid());
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetAllProducts_Returns200()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot"
            };
            
            var createProductRequest2 = new CreateUpdateProductRequest()
            {
                Name = "tomato"
            };
            
            // Given
            var client = factory.CreateClient();
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            await client.PostAsJsonAsync("/api/products", createProductRequest2);
            
            // When
            var response = await client.GetAsync("/api/products");
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<IEnumerable<Product>>();
            result.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task DeleteProduct_WithExistingProduct_Returns200()
        {
            var productName = "carrot";
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName
            };
            
            // Given
            var client = factory.CreateClient();
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<Product>();
            var productId = createdProductResult.Id;
            
            // When
            var response = await client.DeleteAsync("/api/products/" + productId);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task DeleteProduct_WithNonExistingProduct_Returns404()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot"
            };
            
            // Given
            var client = factory.CreateClient();
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var response = await client.DeleteAsync("/api/products/" + Guid.NewGuid());
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task UpdateProduct_WithExistingProduct_Returns200()
        {
            var productName = "carrot";
            var createUpdateProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName
            };
            
            // Given
            var client = factory.CreateClient();
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<Product>();
            var productId = createdProductResult.Id;
            
            // When
            var createUpdateProductRequestUpdated = new CreateUpdateProductRequest()
            {
                Name = productName + " updated"
            };
            var response = await client.PutAsJsonAsync("/api/products/" + productId, createUpdateProductRequestUpdated);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadAsAsync<Product>();
            getResult.Id.Should().Be(productId);
            getResult.Name.Should().Be(productName + " updated");
        }
        
        [Fact]
        public async Task UpdateProduct_WithNonExistingProduct_Returns404()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot"
            };
            
            // Given
            var client = factory.CreateClient();
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var createUpdateProductRequestUpdated = new CreateUpdateProductRequest()
            {
                Name = "carrot updated"
            };
            var response = await client.PutAsJsonAsync("/api/products/" + Guid.NewGuid(), createUpdateProductRequestUpdated);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}