using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ForkEat.Web.Tests
{
    [Collection("Database Test")]
    public abstract class DatabaseTest : IAsyncLifetime
    {
        protected ApplicationDbContext context;
        private readonly IList<string> tableToClear;

        protected DatabaseTest(IList<string> tableToClear)
        {
            this.tableToClear = tableToClear;
        }

        public abstract Task InitializeAsync();

        public async Task DisposeAsync()
        {
            foreach (var table in tableToClear)
            {
                await context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{table}\"");
            }
        }
    }
}