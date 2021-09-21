using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{
    public class FilesRepositoryTests : IAsyncLifetime
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
            await this.context.Database.ExecuteSqlRawAsync("DELETE FROM \"Files\"");
        }

        [Fact]
        public async Task GetFile_ExistingFile_ReturnsFile()
        {
            // Given
            var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
            var dbFile = new DbFile(){Data = file, Type = "gif", Name = "test-file"};
            
            await this.context.Files.AddAsync(dbFile);
            await this.context.SaveChangesAsync();

            IFilesRepository repository = new FilesRepository(this.context);

            // When
            var result = await repository.GetFile(dbFile.Id);

            // Then
            result.Data.Should().BeEquivalentTo(file);
            result.Type.Should().Be("gif");
            result.Id.Should().Be(dbFile.Id);
            result.Name.Should().Be("test-file");
        }
        
        [Fact]
        public async Task GetFile_NonExistingFile_ReturnsFile()
        {
            // Given
            IFilesRepository repository = new FilesRepository(this.context);

            // When
            var result = await repository.GetFile(Guid.NewGuid());
            result.Should().BeNull();
        }

        [Fact]
        public async Task InsertFile_CreatesFileInDb()
        {
            // Given
            var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
            var dbFile = new DbFile(){Data = file, Type = "gif", Name = "test-file"};
            
            IFilesRepository repository = new FilesRepository(this.context);

            // When
            DbFile result = await repository.InsertFile(dbFile);
            
            // When
            result.Data.Should().BeEquivalentTo(file);
            result.Type.Should().Be("gif");
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("test-file");

            var fileInDb = await this.context.Files.FirstAsync(file => file.Id == result.Id);
            fileInDb.Data.Should().BeEquivalentTo(file);
            fileInDb.Type.Should().Be("gif");
            fileInDb.Id.Should().Be(result.Id);
            fileInDb.Name.Should().Be("test-file");
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

            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
        }

    }
}