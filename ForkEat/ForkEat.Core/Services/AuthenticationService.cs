using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository repository;

        public AuthenticationService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public Task<User> Login(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Register(RegisterUserRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}