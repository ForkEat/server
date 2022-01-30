using System.Threading.Tasks;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories;

public interface IUserRepository
{
    Task<User> FindUserByEmail(string email);
    Task<User> InsertUser(User user);
    Task<bool> UserExistsByEmail(string userEmail);
}