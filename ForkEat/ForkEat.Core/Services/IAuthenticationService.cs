using System.Threading.Tasks;
using ForkEat.Core.Contracts;

namespace ForkEat.Core.Services;

public interface IAuthenticationService
{
    Task<LoginUserResponse> Login(LoginUserRequest request);
    Task<RegisterUserResponse> Register(RegisterUserRequest request);
}