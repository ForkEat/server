using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using ForkEat.Web.Tests.TestAssets;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace ForkEat.Web.Tests
{
    [Collection("Database Test")]
    public abstract class DatabaseTest : IAsyncLifetime
    {
        protected ApplicationDbContext context;
        protected DataFactory dataFactory;
        
        private readonly IList<string> tableToClear;

        protected DatabaseTest(IList<string> tableToClear)
        {
            this.tableToClear = tableToClear;
        }

        public abstract ApplicationDbContext GetDbContext();

        public DbContextOptions GetOptionsForOtherDbContext()
        {
            var databaseUri = new Uri(Environment.GetEnvironmentVariable("TEST_DATABASE_URL"));
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };
            var options = new DbContextOptionsBuilder()
                .UseNpgsql(builder.ToString())
                .Options;
            return options;
        }
        
        public virtual async Task InitializeAsync()
        {
            this.context = this.GetDbContext();
            this.dataFactory = new DataFactory(this.context);
            await this.context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            foreach (var table in tableToClear)
            {
                await context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{table}\"");
            }
        }
    }
}