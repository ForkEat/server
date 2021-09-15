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
    public class AuthenticationTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> factory;

        public AuthenticationTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task RegisterAndLogin()
        {
            // Given
            var client = factory.CreateClient();
            var registerUserRequest = new RegisterUserRequest()
            {
                Email = "toto@gmail.com",
                Password = "bonjour",
                UserName = "toto"
            };

            // When
            var response = await client.PostAsJsonAsync("/api/auth/register", registerUserRequest);
            
            // Then
            var result = await response.Content.ReadAsAsync<User>();
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Id.Should().NotBe(null);
            result.UserName = "";
        }

        public void Dispose()
        {
        }
    }
}