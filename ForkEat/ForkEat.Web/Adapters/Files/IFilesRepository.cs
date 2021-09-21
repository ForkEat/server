using System;
using System.Threading.Tasks;

namespace ForkEat.Web.Adapters.Files
{
    public interface IFilesRepository
    {
        Task<DbFile> GetFile(Guid fileId);
        Task<DbFile> InsertFile(DbFile dbFile);
    }
}