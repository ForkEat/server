using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForkEat.Core.Repositories
{
    public interface ILikeRepository
    {
        Task<bool> LikeRecipe(Guid userId, Guid recipeId);
        Task UnlikeRecipe(Guid userId, Guid recipeId);
        Task<bool> GetLike(Guid userId, Guid recipeId);
        Task<List<Guid>> GetLikes(Guid userId, List<Guid> recipeIds);
    }
}