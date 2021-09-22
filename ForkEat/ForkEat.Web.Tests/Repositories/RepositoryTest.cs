using System;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ForkEat.Web.Tests.Repositories
{
    public abstract class RepositoryTest : DatabaseTest
    {
        protected RepositoryTest(string tableToClear) : base(tableToClear)
        {
        }

        public override async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(GetPostgresConnectionString())
                .Options;

            this.context = new ApplicationDbContext(options);

            await this.context.Database.MigrateAsync();
        }


        private string GetPostgresConnectionString()
        {
            var databaseUrl = Environment.GetEnvironmentVariable("TEST_DATABASE_URL");
            if (databaseUrl is null)
            {
                throw new ArgumentException("Please populate the TEST_DATABASE_URL env variable");
            }

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
        }
    }
}