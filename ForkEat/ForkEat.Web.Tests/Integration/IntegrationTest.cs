using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Web.Adapters.Json;
using ForkEat.Web.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForkEat.Web.Tests.Integration
{
    public abstract class IntegrationTest : DatabaseTest, IClassFixture<WebApplicationFactory<Startup>>
    {
        protected HttpClient client;
        private readonly WebApplicationFactory<Startup> factory;
        protected readonly MediaTypeFormatterCollection Formatters = new MediaTypeFormatterCollection(); 
        
        protected IntegrationTest(WebApplicationFactory<Startup> factory, IList<string> tableToClear) : base(tableToClear)
        {
            this.factory = factory;
            Formatters.JsonFormatter.SerializerSettings.Converters.Add(new DateOnlyJsonConverter());
        }

        public override ApplicationDbContext GetDbContext()
        {
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException("Service Scope Factory is null");
            var scope = scopeFactory.CreateScope();
            return scope.ServiceProvider.GetService<ApplicationDbContext>();
        }

        public override async Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("DATABASE_URL",
                Environment.GetEnvironmentVariable("TEST_DATABASE_URL") ??
                throw new ArgumentException("Please populate TEST_DATABASE_URL env variable"));
            client = factory.CreateClient();
            await base.InitializeAsync();
        }

        protected async Task<string> RegisterAndLogin()
        {
            var registerUserRequest = new RegisterUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "Bonj@ur42",
                UserName = "toto"
            };

            await client.PostAsJsonAsync("/api/auth/register", registerUserRequest);

            var loginUser = new LoginUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "Bonj@ur42"
            };
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginUser);
            var loginResult = await loginResponse.Content.ReadAsAsync<LoginUserResponse>();

            return loginResult.Token;
        }
    }
}