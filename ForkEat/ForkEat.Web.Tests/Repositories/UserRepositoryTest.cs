using System;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class UserRepositoryTest : IAsyncLifetime
    {
        private ApplicationDbContext context;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(GetPostgresConnectionString())
                .Options;
            
            this.context = new ApplicationDbContext(options);

            await this.context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\"");
        }
        
        private string GetPostgresConnectionString()
        {
            var databaseUrl = Environment.GetEnvironmentVariable("TEST_DATABASE_URL");
            if (databaseUrl is null)
            {
                throw new ArgumentException("Please populate the TEST_DATABASE_URL env variable");
            }

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
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
            result.Id.Should().NotBe(Guid.Empty);
            result.Email.Should().Be("john.shepard@sr2-normandy.com");
            result.UserName.Should().Be("John Shepard");
            result.Password.Should().Be(hashedPassword);

            var userInDb = await this.context.Users.FirstAsync();
            userInDb.Id.Should().Be(result.Id);
            userInDb.Email.Should().Be("john.shepard@sr2-normandy.com");
            userInDb.UserName.Should().Be("John Shepard");
            userInDb.Password.Should().Be(hashedPassword);
        }
    }
}