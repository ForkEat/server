using System;
using System.Net.Http;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForkEat.Web.Tests
{
    public abstract class IntegrationTest : DatabaseTest, IClassFixture<WebApplicationFactory<Startup>>
    {
        protected HttpClient client;
        protected readonly WebApplicationFactory<Startup> factory;


        protected IntegrationTest(WebApplicationFactory<Startup> factory, string tableToClear) : base(tableToClear)
        {
            this.factory = factory;
        }

        public override async Task InitializeAsync()
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
    }
}