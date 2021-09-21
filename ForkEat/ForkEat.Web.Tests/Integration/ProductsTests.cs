using System;
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
            var productName = "carrott";


            // Given
            var client = factory.CreateClient();
            var createProductRequest = new CreateProductRequest()
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
            var productName = "carrott";
            var createProductRequest = new CreateProductRequest()
            {
                Name = productName
            };
            
            Environment.SetEnvironmentVariable("DATABASE_URL", Environment.GetEnvironmentVariable("TEST_DATABASE_URL") ?? throw new ArgumentException("Please populate TEST_DATABASE_URL env variable"));

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
            var createProductRequest = new CreateProductRequest()
            {
                Name = "carrott"
            };
            
            Environment.SetEnvironmentVariable("DATABASE_URL", Environment.GetEnvironmentVariable("TEST_DATABASE_URL") ?? throw new ArgumentException("Please populate TEST_DATABASE_URL env variable"));

            // Given
            var client = factory.CreateClient();
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var response = await client.GetAsync("/api/products/" + Guid.NewGuid());
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}