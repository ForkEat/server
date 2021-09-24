using System.Linq;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests
{
    public abstract class DatabaseTest : IAsyncLifetime
    {
        protected ApplicationDbContext context;
        private readonly string[] tableToClear;

        protected DatabaseTest(string[] tableToClear)
        {
            this.tableToClear = tableToClear;
        }

        public abstract Task InitializeAsync();

        public async Task DisposeAsync()
        {
            await Task.WhenAll(tableToClear.Select(table => context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{table}\"")));
        }
    }
}