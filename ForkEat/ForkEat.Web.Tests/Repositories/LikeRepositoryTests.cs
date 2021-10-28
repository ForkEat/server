using System;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database.Entities;
using ForkEat.Web.Database.Repositories;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class LikeRepositoryTests : RepositoryTest
    {
        public LikeRepositoryTests() : base(new string[] { "Likes", "Recipes", "Users"})
        {
        }

        [Fact]
        public async Task LikeRecipe_ReturnsTrue()
        {
            // Given
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var user = new User()
            {
                Id = userId
            };

            var recipe = new RecipeEntity()
            {
                Id = recipeId
            };

            await context.Users.AddAsync(user);
            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            var repository = new LikeRepository(context);

            // When
            var result = await repository.LikeRecipe(userId, recipeId);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UnlikeRecipe_Ok()
        {
            // Given
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var user = new User()
            {
                Id = userId
            };

            var recipe = new RecipeEntity()
            {
                Id = recipeId
            };

            await context.Users.AddAsync(user);
            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            var repository = new LikeRepository(context);
            await repository.LikeRecipe(userId, recipeId);
            context.Likes.Should().HaveCount(1);

            // When
            await repository.UnlikeRecipe(userId, recipeId);

            // Then
            context.Likes.Should().BeEmpty();
        }

        [Fact]
        public async Task IsRecipeAlreadyLiked_ReturnsTrue()
        {
            // Given
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var user = new User()
            {
                Id = userId
            };

            var recipe = new RecipeEntity()
            {
                Id = recipeId
            };

            await context.Users.AddAsync(user);
            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            var repository = new LikeRepository(context);
            await repository.LikeRecipe(userId, recipeId);
            context.Likes.Should().HaveCount(1);

            // When
            var result = await repository.GetLike(userId, recipeId);

            // Then
            result.Should().BeTrue();
        }
    }
}