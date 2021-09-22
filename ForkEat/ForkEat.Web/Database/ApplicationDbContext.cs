﻿using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<DbFile> Files { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<RecipeEntity> Recipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(user => user.Id);
            modelBuilder.Entity<User>().Property(user => user.Email);
            modelBuilder.Entity<User>().Property(user => user.Password);
            modelBuilder.Entity<User>().Property(user => user.UserName);

            modelBuilder.Entity<Product>().HasKey(product => product.Id);
            modelBuilder.Entity<Product>().Property(product => product.Name);

            modelBuilder.Entity<DbFile>().HasKey(file => file.Id);
            modelBuilder.Entity<DbFile>().Property(file => file.Type);
            modelBuilder.Entity<DbFile>().Property(file => file.Data);
            modelBuilder.Entity<DbFile>().Property(file => file.Name);

            modelBuilder.Entity<Unit>().HasKey(unit => unit.Id);
            modelBuilder.Entity<Unit>().Property(unit => unit.Name);
            modelBuilder.Entity<Unit>().Property(unit => unit.Symbol);

            modelBuilder.Entity<RecipeEntity>().HasKey(recipe => recipe.Id);
            modelBuilder.Entity<RecipeEntity>().Property(recipe => recipe.Name);
            modelBuilder.Entity<RecipeEntity>().Property(recipe => recipe.Difficulty);
            modelBuilder.Entity<RecipeEntity>().HasMany<StepEntity>(recipe => recipe.Steps).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RecipeEntity>().HasMany<IngredientEntity>(recipe => recipe.Ingredients).WithOne().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StepEntity>().HasKey(step => step.Id);
            modelBuilder.Entity<StepEntity>().Property(step => step.Name);
            modelBuilder.Entity<StepEntity>().Property(step => step.Instructions);
            modelBuilder.Entity<StepEntity>().Property(step => step.EstimatedTime);

            modelBuilder.Entity<IngredientEntity>().HasKey(ingredient => ingredient.Id);
            modelBuilder.Entity<IngredientEntity>().Property(ingredient => ingredient.Quantity);
            modelBuilder.Entity<IngredientEntity>().HasOne<Product>(ingredient => ingredient.Product).WithMany();
        }
    }
}