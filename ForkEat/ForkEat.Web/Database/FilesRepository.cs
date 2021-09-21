using System;
using System.Threading.Tasks;
using ForkEat.Web.Adapters.Files;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database
{
    public class FilesRepository : IFilesRepository
    {
        private readonly ApplicationDbContext context;

        public FilesRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<DbFile> GetFile(Guid fileId)
        {
            return this.context.Files.FirstOrDefaultAsync(file => file.Id == fileId);
        }

        public async Task<DbFile> InsertFile(DbFile dbFile)
        {
            await this.context.AddAsync(dbFile);
            await this.context.SaveChangesAsync();
            return dbFile;
        }
    }
}