﻿// <auto-generated />
using System;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ForkEat.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220219164635_pdtType3")]
    partial class pdtType3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ForkEat.Core.Domain.ProductType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("ForkEat.Core.Domain.Unit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Symbol")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("ForkEat.Core.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ForkEat.Web.Adapters.Files.DbFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("Data")
                        .HasColumnType("bytea");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.IngredientEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("UnitId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("RecipeId");

                    b.HasIndex("UnitId");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.LikeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.HasIndex("UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.ProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("ProductTypeId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ProductTypeId1")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductTypeId");

                    b.HasIndex("ProductTypeId1");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.RecipeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("Difficulty")
                        .HasColumnType("bigint");

                    b.Property<Guid>("ImageId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.StepEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("EstimatedTime")
                        .HasColumnType("interval");

                    b.Property<string>("Instructions")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("Order")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("RecipeEntityId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RecipeEntityId");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.StockEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("BestBeforeDate")
                        .HasColumnType("date");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("PurchaseDate")
                        .HasColumnType("date");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<Guid?>("UnitId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UnitId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.IngredientEntity", b =>
                {
                    b.HasOne("ForkEat.Web.Database.Entities.ProductEntity", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ForkEat.Web.Database.Entities.RecipeEntity", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ForkEat.Core.Domain.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId");

                    b.Navigation("Product");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.LikeEntity", b =>
                {
                    b.HasOne("ForkEat.Web.Database.Entities.RecipeEntity", "Recipe")
                        .WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ForkEat.Core.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.ProductEntity", b =>
                {
                    b.HasOne("ForkEat.Core.Domain.ProductType", null)
                        .WithMany()
                        .HasForeignKey("ProductTypeId");

                    b.HasOne("ForkEat.Core.Domain.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId1");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.StepEntity", b =>
                {
                    b.HasOne("ForkEat.Web.Database.Entities.RecipeEntity", null)
                        .WithMany("Steps")
                        .HasForeignKey("RecipeEntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.StockEntity", b =>
                {
                    b.HasOne("ForkEat.Web.Database.Entities.ProductEntity", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("ForkEat.Core.Domain.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId");

                    b.Navigation("Product");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("ForkEat.Web.Database.Entities.RecipeEntity", b =>
                {
                    b.Navigation("Ingredients");

                    b.Navigation("Steps");
                });
#pragma warning restore 612, 618
        }
    }
}
