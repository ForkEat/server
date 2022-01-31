﻿using ForkEat.Core.Domain;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
        
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<DbFile> Files { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<RecipeEntity> Recipes { get; set; }
    public DbSet<StockEntity> Stocks { get; set; }
    public DbSet<IngredientEntity> Ingredients { get; set; }
    public DbSet<StepEntity> Steps { get; set; }
    public DbSet<LikeEntity> Likes { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.Id);
        modelBuilder.Entity<User>().Property(user => user.Email);
        modelBuilder.Entity<User>().Property(user => user.Password);
        modelBuilder.Entity<User>().Property(user => user.UserName);
        modelBuilder.Entity<User>().HasMany<LikeEntity>().WithOne().HasForeignKey(like => like.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductEntity>().HasKey(product => product.Id);
        modelBuilder.Entity<ProductEntity>().Property(product => product.Name);
        modelBuilder.Entity<ProductEntity>().Property(product => product.ImageId);

        modelBuilder.Entity<DbFile>().HasKey(file => file.Id);
        modelBuilder.Entity<DbFile>().Property(file => file.Type);
        modelBuilder.Entity<DbFile>().Property(file => file.Data);
        modelBuilder.Entity<DbFile>().Property(file => file.Name);

        modelBuilder.Entity<Unit>().HasKey(unit => unit.Id);
        modelBuilder.Entity<Unit>().Property(unit => unit.Name);
        modelBuilder.Entity<Unit>().Property(unit => unit.Symbol);

        modelBuilder.Entity<RecipeEntity>().HasKey(recipe => recipe.Id);
        modelBuilder.Entity<RecipeEntity>().Property(recipe => recipe.Name);
        modelBuilder.Entity<RecipeEntity>().Property(recipe => recipe.ImageId);
        modelBuilder.Entity<RecipeEntity>().Property(recipe => recipe.Difficulty);
        modelBuilder.Entity<RecipeEntity>().HasMany<StepEntity>(recipe => recipe.Steps).WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RecipeEntity>().HasMany<IngredientEntity>(recipe => recipe.Ingredients).WithOne()
            .HasForeignKey(i => i.RecipeId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RecipeEntity>().HasMany<LikeEntity>().WithOne().HasForeignKey(like => like.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StepEntity>().HasKey(step => step.Id);
        modelBuilder.Entity<StepEntity>().Property(step => step.Name);
        modelBuilder.Entity<StepEntity>().Property(step => step.Instructions);
        modelBuilder.Entity<StepEntity>().Property(step => step.EstimatedTime);

        modelBuilder.Entity<IngredientEntity>().HasKey(ingredient => ingredient.Id);
        modelBuilder.Entity<IngredientEntity>().Property(ingredient => ingredient.Quantity);
        modelBuilder.Entity<IngredientEntity>().HasOne(ingredient => ingredient.Product).WithMany()
            .HasForeignKey(ingredient => ingredient.ProductId);
        modelBuilder.Entity<IngredientEntity>().HasOne(ingredient => ingredient.Unit).WithMany();

        modelBuilder.Entity<StockEntity>().HasKey(stock => stock.Id);
        modelBuilder.Entity<StockEntity>().Property(stock => stock.Quantity);
        modelBuilder.Entity<StockEntity>().HasOne(stock => stock.Unit).WithMany();
        modelBuilder.Entity<StockEntity>().HasOne(stock => stock.Product).WithMany();
        modelBuilder.Entity<StockEntity>().Property(stock => stock.BestBeforeDate);
        modelBuilder.Entity<StockEntity>().Property(stock => stock.PurchaseDate);

        modelBuilder.Entity<LikeEntity>().HasKey(like => like.Id);
        modelBuilder.Entity<LikeEntity>().HasOne(like => like.User).WithMany().HasForeignKey(like => like.UserId);
        modelBuilder.Entity<LikeEntity>().HasOne(like => like.Recipe).WithMany().HasForeignKey(like => like.RecipeId);

        modelBuilder.Entity<ProductType>().HasKey(productType => productType.Id);
        modelBuilder.Entity<ProductType>().Property(productType => productType.Name);
    }
}