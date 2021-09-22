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
    }
}