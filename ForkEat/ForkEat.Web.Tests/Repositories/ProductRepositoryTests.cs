using System;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class ProductRepositoryTests : IAsyncLifetime
    {
        private ApplicationDbContext context;
        
        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(GetPostgresConnectionString())
                .Options;
            
            this.context = new ApplicationDbContext(options);

            await this.context.Database.MigrateAsync();
        }
        
        private string GetPostgresConnectionString()
        {
            var databaseUrl = Environment.GetEnvironmentVariable("TEST_DATABASE_URL");
            if (databaseUrl is null)
            {
                throw new ArgumentException("Please populate the TEST_DATABASE_URL env variable");
            }

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
        }

        public async Task DisposeAsync()
        {
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Products\"");
        }
        
        [Fact]
        public async Task InsertProduct_InsertRecordInDatabase()
        {
            // Given
            var productName = "carrott";
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = productName
            };
            var repository = new ProductRepository(context);
            
            // When
            var result = await repository.InsertProduct(product);

            // Then
            context.Products.Should().ContainSingle();
            
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);

            var productInDb = await context.Products.FirstAsync(product => product.Id == result.Id);
            productInDb.Id.Should().Be(result.Id);
            productInDb.Name.Should().Be(productName);
        }
    }
}