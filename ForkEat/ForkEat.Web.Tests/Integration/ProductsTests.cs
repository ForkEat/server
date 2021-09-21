using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForkEat.Web.Tests
{
    public class ProductsTests : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Startup> factory;
        private ApplicationDbContext context;
        private HttpClient client;

        public ProductsTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task CreateProduct_withValidParams_Returns201()
        {
            var productName = "carrott";
            
            Environment.SetEnvironmentVariable("DATABASE_URL", Environment.GetEnvironmentVariable("TEST_DATABASE_URL") ?? throw new ArgumentException("Please populate TEST_DATABASE_URL env variable"));

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

        public Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("DATABASE_URL",
                Environment.GetEnvironmentVariable("TEST_DATABASE_URL") ??
                throw new ArgumentException("Please populate TEST_DATABASE_URL env variable"));
            client = factory.CreateClient();
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory?.CreateScope();
            context = scope?.ServiceProvider.GetService<ApplicationDbContext>();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Products\"");
        }
    }
}