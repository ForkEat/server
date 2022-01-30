using System;
using System.IO;
using System.Threading.Tasks;
using ForkEat.Web.Adapters.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFilesRepository repository;

    public FilesController(IFilesRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet("{fileId:Guid}")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public async Task<FileContentResult> DownloadFile(Guid fileId)
    {
        var dbFile = await this.repository.GetFile(fileId);
        return File(dbFile.Data, "application/octet-stream", dbFile.Name);
    }

    [HttpPost]
    public async Task<ActionResult<DbFileResponse>> UploadFile(IFormFile file)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        var dbFile = new DbFile()
        {
            Data = stream.ToArray(),
            Name = file.FileName.Split(".")[0],
            Type = file.FileName.Split(".")[1]
        };

        await this.repository.InsertFile(dbFile);

        return new DbFileResponse() {Id = dbFile.Id, Name = dbFile.Name, Type = dbFile.Type};
    }
}