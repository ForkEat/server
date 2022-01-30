using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Tests.Integration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests.Integration;

public class FilesTests : AuthenticatedTests
{
    public FilesTests(WebApplicationFactory<Startup> factory) : base(factory, new string[]{"Files"})
    {
    }

    [Fact]
    public async Task GetFile_ReturnsFileAsOctetStream()
    {
        // Given

        var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
        var dbFile = new DbFile() { Data = file, Type = "gif", Name = "test-file" };

        await this.context.Files.AddAsync(dbFile);
        await this.context.SaveChangesAsync();

        var id = dbFile.Id;

        // When
        var result = await client.GetAsync($"api/files/{id}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultData = await result.Content.ReadAsByteArrayAsync();
        resultData.Should().BeEquivalentTo(file);
        result.Content.Headers.ContentType.ToString().Should().Be("application/octet-stream");
    }

    [Fact]
    public async Task CreateFile_CreateFileInDbAndReturnsId()
    {
        // Given
        var file = await File.ReadAllBytesAsync("TestAssets/test-file.gif");
        var multipartFormDataContent = new MultipartFormDataContent();
        multipartFormDataContent.Add(new StreamContent(new MemoryStream(file)), "file", "test-file.gif");

        // When
        var response = await client.PostAsync($"api/files", multipartFormDataContent);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsAsync<DbFileResponse>();

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("test-file");
        result.Type.Should().Be("gif");
            
        DbFile dbFile = await this.context.Files.FirstAsync(f => f.Id == result.Id);
        dbFile.Id.Should().NotBeEmpty();
        dbFile.Data.Should().BeEquivalentTo(file);
        dbFile.Name.Should().Be("test-file");
        dbFile.Type.Should().Be("gif");
    }
}