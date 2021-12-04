using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LikeRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> LikeRecipe(Guid userId, Guid recipeId)
        {
            var recipe = dbContext.Recipes.First(r => r.Id == recipeId);

            if (recipe is null)
            {
                throw new RecipeNotFoundException();
            }

            var user = dbContext.Users.First(u => u.Id == userId);

            if (user is null)
            {
                throw new UserNotFoundException();
            }

            var like = new LikeEntity()
            {
                Recipe = recipe,
                User = user
            };

            await dbContext.Likes.AddAsync(like);
            await dbContext.SaveChangesAsync();

            return await dbContext.Likes.FirstAsync(like => like.Recipe.Id.Equals(recipeId) && like.User.Id.Equals(userId)) != null;
        }

        public async Task UnlikeRecipe(Guid userId, Guid recipeId)
        {
            var like = await dbContext.Likes.FirstAsync(like => like.Recipe.Id.Equals(recipeId) && like.User.Id.Equals(userId));

            dbContext.Likes.Remove(like);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> GetLike(Guid userId, Guid recipeId)
        {
            return await dbContext.Likes.FirstOrDefaultAsync(like => like.Recipe.Id.Equals(recipeId) && like.User.Id.Equals(userId)) != null;
        }

        public Task<List<Guid>> GetLikes(Guid userId, List<Guid> recipeIds)
        {
            return Task.FromResult(dbContext.Likes.Where(like => recipeIds.Contains(like.Recipe.Id) && like.User.Id.Equals(userId))
                .Select(like => like.Id)
                .ToList());
        }
    }
}