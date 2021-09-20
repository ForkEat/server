using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ForkEat.Core.Tests
{
    public class AuthenticationServiceTests
    {
        [Fact]
        public async Task Register_CreatesUserInRepo()
        {
            // Given
            var registerUserRequest = new RegisterUserRequest()
            {
                UserName = "Toto",
                Email = "toto@email.fr",
                Password = "Bonj@ur42"
            };

            var repoMock = new Mock<IUserRepository>();
            var validatorMock = new Mock<IPasswordValidator>();

            User insertedUser = null;
            repoMock.Setup(mock => mock.InsertUser(It.IsAny<User>()))
                .Returns<User>(user =>
                {
                    insertedUser = user;
                    insertedUser.Id = Guid.NewGuid();
                    return Task.FromResult(insertedUser);
                });

            validatorMock.Setup(mock => mock.Validate(It.IsAny<User>()))
                .Returns(new ValidationResult());

            var service = new AuthenticationService(repoMock.Object, validatorMock.Object);
            
            // When
            var user = await service.Register(registerUserRequest);
            
            // Then
            user.Email.Should().Be("toto@email.fr");
            user.UserName.Should().Be("Toto");

            insertedUser.Id.Should().NotBe(Guid.Empty);
            insertedUser.Email.Should().Be("toto@email.fr");
            insertedUser.UserName.Should().Be("Toto");
            insertedUser.Password.Should().Be("Bonj@ur42");
        }

        [Fact]
        public async Task Login_ExistingUserValidCredentials_ReturnsToken()
        {
            var loginUserRequest = new LoginUserRequest()
            {
                Email = "toto@email.fr",
                Password = "Bonj@ur42"
            };

            var secret = "bonjourlemondecestvraimentchouetteajd";
            Environment.SetEnvironmentVariable("JWT_SECRET", secret);

            var userId = Guid.NewGuid();
            
            var repoMock = new Mock<IUserRepository>();
            var validatorMock = new Mock<IPasswordValidator>();
            repoMock.Setup(x => x.FindUserByEmail("toto@email.fr"))
                .Returns(() => Task.FromResult(new User()
                {
                    Id = userId,
                    Email = "toto@email.fr",
                    UserName = "Toto",
                    Password = BCrypt.Net.BCrypt.HashPassword("Bonj@ur42")
                }));
            var service = new AuthenticationService(repoMock.Object, validatorMock.Object);
            var user = await service.Login(loginUserRequest);
            
            user.Token.Should().NotBe(String.Empty);
            user.Email.Should().Be("toto@email.fr");
            user.UserName.Should().Be("Toto");
            
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                            throw new ArgumentException("JWT_SECRET Env variable is not set");
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            
            var principal = tokenHandler.ValidateToken(user.Token, validationParameters, out _);

            principal.Claims.ToList()[0].Subject.Name.Should().Be( userId.ToString());
            
        }

    }

}