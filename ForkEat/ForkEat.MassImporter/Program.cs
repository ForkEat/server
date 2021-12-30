using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

IEnumerable<string> lines = File.ReadLines(@"RAW_recipes_fixed.csv");

bool IsLineValid(string[] line)
{
    try
    {
        return line[8].Contains("[")
               && line[8].Contains("]")
               && line[8].Count(c => c == ',') == int.Parse(line[7]) - 1
               && line[0] != ""
               && line[1] != ""
               && line[2] != ""
               && line[3] != "";
    }
    catch (Exception e)
    {
        return false;
    }
}

RecipeEntity ParseRecipe(string[] line)
{
    string name = line[0];
    string stepsJsonString = line[8]
        .Replace("'", "\"")
        .Replace("\n", "")
        .Trim('\"');

    TimeSpan stepTime = TimeSpan.FromMinutes(double.Parse(line[2]) / int.Parse(line[7]));
    List<StepEntity>? steps = null;
    try
    {
        string[] stepsLines = JsonSerializer.Deserialize<string[]>(stepsJsonString);
        uint i = 0;
        steps = stepsLines
            .Select(step => new StepEntity {Instructions = step, Order = i++, EstimatedTime = stepTime })
            .ToList();
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e.Message);
    }

    return new RecipeEntity()
    {
        Name = name,
        Difficulty = 2,
        Steps = steps ?? new List<StepEntity>(),
    };
}

List<RecipeEntity> recipes = lines
    .Skip(1)
    .Select(line => line.Split(";"))
    .Where(IsLineValid)
    .Select(ParseRecipe)
    .ToList();

Console.WriteLine($"Processed {recipes.Count} records");

var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = "localhost",
    Port = 5432,
    Username = "postgres",
    Password = "mysecretpassword",
    Database = "forkeat"
};
var options = new DbContextOptionsBuilder()
    .UseNpgsql(npgsqlConnectionStringBuilder.ToString())
    .Options;

await using var migrationContext = new ApplicationDbContext(options);
await migrationContext.Database.MigrateAsync();

await Task.WhenAll(recipes.Chunk(5000)
    .Select(async chunk =>
    {
        await using var context = new ApplicationDbContext(options);
        await context.Recipes.AddRangeAsync(chunk);
        await context.SaveChangesAsync();
        Console.WriteLine("Inserted one chunk");
    }));