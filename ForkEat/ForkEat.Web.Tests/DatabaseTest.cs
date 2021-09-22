using System.Threading.Tasks;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests
{
    public abstract class DatabaseTest : IAsyncLifetime
    {
        protected ApplicationDbContext context;
        private readonly string tableToClear;

        protected DatabaseTest(string tableToClear)
        {
            this.tableToClear = tableToClear;
        }

        public abstract Task InitializeAsync();

        public async Task DisposeAsync()
        {
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableToClear}\"");
        }
    }
}