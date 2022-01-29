using System;
using ForkEat.Core.Domain;

namespace ForkEat.Web.Database.Entities
{
    public class LikeEntity
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public RecipeEntity Recipe { get; set; }
        public Guid RecipeId { get; set; }
    }
}