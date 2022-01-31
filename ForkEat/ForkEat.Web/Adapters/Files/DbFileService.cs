using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ForkEat.Web.Adapters.Files
{
    public class DbFileService
    {
        private readonly IFilesRepository repository;

        public DbFileService(IFilesRepository repository)
        {
            this.repository = repository;
        }

        public async Task<DbFileResponse> InsertFileInDb(IFormFile file)
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

        public async Task DeleteFile(Guid oldImageId)
        {
            await this.repository.DeleteFile(oldImageId);
        }
    }
}