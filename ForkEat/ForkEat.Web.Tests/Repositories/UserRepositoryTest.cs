using System;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories;

public class UserRepositoryTest : RepositoryTest
{
    public UserRepositoryTest() : base(new string[]{"Users"})
    {
    }

    [Fact]
    public async Task UserExistsByEmail_ExistingUser_ReturnsTrue()
    {
        // Given
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("test");
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = "john.shepard@sr2-normandy.com",
            UserName = "John Shepard",
            Password = hashedPassword
        };
            
        await this.context.Users.AddAsync(user);
        await this.context.SaveChangesAsync();
        var repository = new UserRepository(this.context);
            
        // When
        bool exists = await repository.UserExistsByEmail("john.shepard@sr2-normandy.com");
            
        // Then
        exists.Should().BeTrue();
    }
        
    [Fact]
    public async Task UserExistsByEmail_NonExistingUser_ReturnsFalse()
    {
        // Given
        var repository = new UserRepository(this.context);
            
        // When
        bool exists = await repository.UserExistsByEmail("john.shepard@sr2-normandy.com");
            
        // Then
        exists.Should().BeFalse();
    }
        
    [Fact]
    public async Task FindUserByEmail_ExistingUser_ReturnsUser()
    {
        // Given
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("test");
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = "john.shepard@sr2-normandy.com",
            UserName = "John Shepard",
            Password = hashedPassword
        };

        await this.context.Users.AddAsync(user);
        await this.context.SaveChangesAsync();
        var repository = new UserRepository(this.context);

        // When
        var result = await repository.FindUserByEmail("john.shepard@sr2-normandy.com");

        // Then
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be("john.shepard@sr2-normandy.com");
        result.UserName.Should().Be("John Shepard");
        result.Password.Should().Be(hashedPassword);
    }

    [Fact]
    public async Task FindUserByEmail_NonExistingUser_ReturnsNull()
    {
        // Given
        var repository = new UserRepository(this.context);

        // When
        var result = await repository.FindUserByEmail("john.shepard@sr2-normandy.com");

        // Then
        result.Should().Be(null);
    }

    [Fact]
    public async Task InsertUser_InsertRecordInDatabase()
    {
        // Given
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("test");
        var user = new User()
        {
            Email = "john.shepard@sr2-normandy.com",
            UserName = "John Shepard",
            Password = hashedPassword
        };
        var repository = new UserRepository(this.context);

        // When
        var result = await repository.InsertUser(user);

        // Then
        context.Users.Should().ContainSingle();

        result.Id.Should().NotBe(Guid.Empty);
        result.Email.Should().Be("john.shepard@sr2-normandy.com");
        result.UserName.Should().Be("John Shepard");
        result.Password.Should().Be(hashedPassword);

        var userInDb = await this.context.Users.FirstAsync(u => u.Id == result.Id);
        userInDb.Id.Should().Be(result.Id);
        userInDb.Email.Should().Be("john.shepard@sr2-normandy.com");
        userInDb.UserName.Should().Be("John Shepard");
        userInDb.Password.Should().Be(hashedPassword);
    }
}