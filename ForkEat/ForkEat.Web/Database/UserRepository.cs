using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<User> FindUserByEmail(string email)
        {
            return this.dbContext
                .Users
                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<User> InsertUser(User user)
        {
            await this.dbContext.Users.AddAsync(user);
            await this.dbContext.SaveChangesAsync();
            return user;
        }
    }
}