using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Repositories
{

    public class FilesRepositoryTests : RepositoryTest
    {
        public FilesRepositoryTests() : base(new string[]{"Files"})
        {
        }


        [Fact]
        public async Task GetFile_ExistingFile_ReturnsFile()
        {
            // Given
            var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
            var dbFile = new DbFile() { Data = file, Type = "gif", Name = "test-file" };

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
            var dbFile = new DbFile() { Data = file, Type = "gif", Name = "test-file" };

            IFilesRepository repository = new FilesRepository(this.context);

            // When
            DbFile result = await repository.InsertFile(dbFile);

            // When
            result.Data.Should().BeEquivalentTo(file);
            result.Type.Should().Be("gif");
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("test-file");

            var fileInDb = await this.context.Files.FirstAsync(f => f.Id == result.Id);
            fileInDb.Data.Should().BeEquivalentTo(file);
            fileInDb.Type.Should().Be("gif");
            fileInDb.Id.Should().Be(result.Id);
            fileInDb.Name.Should().Be("test-file");
        }
    }
}