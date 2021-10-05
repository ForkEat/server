using System;
using System.IO;
using System.Threading.Tasks;
using ForkEat.Web.Adapters.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
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
        public async Task UploadFile(IFormFile file)
        {
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            await this.repository.InsertFile(new DbFile()
            {
                Data = stream.ToArray(),
                Name = file.FileName.Split(".")[0],
                Type = file.FileName.Split(".")[1]
            });
        }
    }
}