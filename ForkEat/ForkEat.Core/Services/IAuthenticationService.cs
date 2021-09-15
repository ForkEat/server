using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IAuthenticationService
    {
        Task<User> Login(string email, string password);
        Task<User> Register(RegisterUserRequest request);
    }
}