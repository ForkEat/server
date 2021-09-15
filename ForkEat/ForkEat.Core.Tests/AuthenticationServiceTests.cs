using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
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
                Password = "bonjour"
            };

            var repoMock = new Mock<IUserRepository>();
            User insertedUser = null;
            repoMock.Setup(mock => mock.InsertUser(It.IsAny<User>()))
                .Returns<User>(user =>
                {
                    insertedUser = user;
                    return Task.FromResult(user);
                });

            var service = new AuthenticationService(repoMock.Object);
            
            // When
            var user = await service.Register(registerUserRequest);
            
            // Then
            user.Email.Shoul
        }
    }
}