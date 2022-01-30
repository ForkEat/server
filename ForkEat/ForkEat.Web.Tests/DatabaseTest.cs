using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using ForkEat.Web.Tests.TestAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace ForkEat.Web.Tests;

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