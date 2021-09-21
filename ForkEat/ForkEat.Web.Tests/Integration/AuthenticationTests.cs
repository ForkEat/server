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
    public class AuthenticationTests : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Startup> factory;
        private ApplicationDbContext context;
        private HttpClient client;

        public AuthenticationTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task RegisterAndLogin_WithGoodCredentials_RegistersUsersAndGetToken()
        {
            /*REGISTER*/
            // Given
            var registerUserRequest = new RegisterUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "Bonj@ur42",
                UserName = "toto"
            };

            // When
            var response = await client.PostAsJsonAsync("/api/auth/register", registerUserRequest);

            // Then
            var result = await response.Content.ReadAsAsync<User>();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Id.Should().NotBe(Guid.Empty);
            result.UserName.Should().Be("toto");
            result.Email.Should().Be("toto@gmail.com");


            /*LOGIN*/
            //Given
            var loginUser = new LoginUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "Bonj@ur42"
            };

            //When
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginUser);

            //Then
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadAsAsync<LoginUserResponse>();
            loginResult.Token.Should().NotBe(String.Empty);
            loginResult.UserName.Should().Be("toto");
            loginResult.Email.Should().Be("toto@gmail.com");
        }

        [Fact]
        public async Task RegisterAndLogin_LoginWithGoodCredentials_RegistersUsersAndReturns401()
        {
            /*REGISTER*/
            // Given
            var registerUserRequest = new RegisterUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "Bonj@ur42",
                UserName = "toto"
            };

            // When
            var response = await client.PostAsJsonAsync("/api/auth/register", registerUserRequest);

            // Then
            var result = await response.Content.ReadAsAsync<User>();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Id.Should().NotBe(Guid.Empty);
            result.UserName.Should().Be("toto");
            result.Email.Should().Be("toto@gmail.com");


            /*LOGIN*/
            //Given
            var loginUser = new LoginUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "wrong password"
            };

            // When
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginUser);

            // Then
            loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
            using var scope = scopeFactory.CreateScope();
            context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\"");
        }
    }
}