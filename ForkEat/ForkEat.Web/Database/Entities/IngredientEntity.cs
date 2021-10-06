﻿using System;
using ForkEat.Core.Domain;

namespace ForkEat.Web.Database.Entities
{
    public class IngredientEntity
    {
        public Guid Id { get; set; }
        public ProductEntity Product { get; set; }
        public uint Quantity { get; set; }
        public Unit Unit { get; set; }
        public Guid RecipeId { get; set; }
    }
}