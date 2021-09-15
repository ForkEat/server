using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Services
{
    public interface IAuthenticationService
    {
        Task<LoginUserResponse> Login(LoginUserRequest request);
        Task<User> Register(RegisterUserRequest request);
    }
}