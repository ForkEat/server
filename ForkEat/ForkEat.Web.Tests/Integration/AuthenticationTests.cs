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
            /*REGISTER*/
            // Given
            var client = factory.CreateClient();
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

        public void Dispose()
        {
        }
    }
}