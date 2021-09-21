using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForkEat.Web.Tests
{
    public class FilesTests: IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Startup> factory;
        private ApplicationDbContext context;
        private HttpClient client;

        public FilesTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        public async Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("DATABASE_URL",
                Environment.GetEnvironmentVariable("TEST_DATABASE_URL") ??
                throw new ArgumentException("Please populate TEST_DATABASE_URL env variable"));
            client = factory.CreateClient();
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory?.CreateScope();
            context = scope?.ServiceProvider.GetService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Files\"");
        }
        
        [Fact]
        public async Task GetFile_ReturnsFileAsOctetStream()
        {
            // Given
            
            var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
            var dbFile = new DbFile(){Data = file, Type = "gif", Name = "test-file"};
            
            await this.context.Files.AddAsync(dbFile);
            await this.context.SaveChangesAsync();

            var id = dbFile.Id;
            
            // When
            var result = await client.GetAsync($"api/files/{id}");

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultData = await result.Content.ReadAsByteArrayAsync();
            resultData.Should().BeEquivalentTo(file);
            result.Content.Headers.ContentType.ToString().Should().Be("application/octet-stream");
        }
        
        [Fact]
        public async Task CreateFile_CreateFileInDb()
        {
            // Given
            
            var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(new StreamContent(new MemoryStream(file)), "file", "test-file.gif");
            
            // When
            
            var result = await client.PostAsync($"api/files", multipartFormDataContent);

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            DbFile dbFile = await this.context.Files.FirstAsync();
            dbFile.Id.Should().NotBeEmpty();
            dbFile.Data.Should().BeEquivalentTo(file);
            dbFile.Name.Should().Be("test-file");
            dbFile.Type.Should().Be("gif");
        }
    }
}