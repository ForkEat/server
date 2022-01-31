using System.Text.Json;
using ForkEat.Core.Domain;
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

List<StepEntity>? GetSteps(string stepsJsonString, TimeSpan stepTime)
{
    try
    {
        string[] stepsLines = JsonSerializer.Deserialize<string[]>(stepsJsonString);
        uint i = 0;
        List<StepEntity>? steps = stepsLines
            .Select(step => new StepEntity {Instructions = step, Order = i++, EstimatedTime = stepTime})
            .ToList();

        return steps;
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e.Message);
        return null;
    }
}

Dictionary<string, ProductEntity> GetProducts(IEnumerable<string> line) =>
    line.Select(l => l.Split(";"))
        .Select(l => l[10])
        .Select(ingredientColumn => ingredientColumn.Replace("'", "\"")
            .Replace("\n", "")
            .Trim('\"'))
        .SelectMany(ingredientColumn => JsonSerializer.Deserialize<string[]>(ingredientColumn))
        .Select(ingredientName => new ProductEntity() {Id = Guid.NewGuid(), Name = ingredientName})
        .ToDictionary(product => product.Name, product => product);


string SanitizeJsonColumn(string json) => json
    .Replace("'", "\"")
    .Replace("\n", "")
    .Trim('\"');

List<IngredientEntity> GetIngredients(string[] ingredientNames, Dictionary<string, ProductEntity> products)
{
    return ingredientNames.Select(ingredientName =>
    {
        ProductEntity product = null;
        if (products.ContainsKey(ingredientName))
        {
            product = products[ingredientName];
        }
        else
        {
            product = new ProductEntity() {Id = Guid.NewGuid(), Name = ingredientName};
            products[ingredientName] = product;
        }

        return new IngredientEntity() {Id = Guid.NewGuid(), ProductId = product.Id};
    }).ToList();
}

RecipeEntity ParseRecipe(string[] line, Dictionary<string, ProductEntity> products)
{
    string name = line[0];
    string stepsJsonString = SanitizeJsonColumn(line[8]);
    string ingredientsJsonString = SanitizeJsonColumn(line[10]);

    string[] ingredientNames = JsonSerializer.Deserialize<string[]>(ingredientsJsonString);
    TimeSpan stepTime = TimeSpan.FromMinutes(double.Parse(line[2]) / int.Parse(line[7]));


    return new RecipeEntity()
    {
        Name = name,
        Difficulty = 2,
        Steps = GetSteps(stepsJsonString, stepTime),
        Ingredients = GetIngredients(ingredientNames, products)
    };
}

Dictionary<string, ProductEntity> products = GetProducts(lines);

List<RecipeEntity> recipes = lines
    .Skip(1)
    .Select(line => line.Split(";"))
    .Where(IsLineValid)
    .Select(line => ParseRecipe(line, products))
    .ToList();

Console.WriteLine($"Processed {recipes.Count} records");

var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = "arsenelapostolet.fr",
    Port = 5432,
    Username = "postgres",
    Password = "arsene10091999",
    Database = "forkeat-ml"
};
var options = new DbContextOptionsBuilder()
    .UseNpgsql(npgsqlConnectionStringBuilder.ToString())
    .Options;

await using var migrationContext = new ApplicationDbContext(options);
await migrationContext.Database.MigrateAsync();

await Task.WhenAll(products.Values.Chunk(5000)
    .Select(async chunk =>
    {
        await using var context = new ApplicationDbContext(options);
        await context.Products.AddRangeAsync(chunk);
    }));


await Task.WhenAll(recipes.Chunk(5000)
    .Select(async chunk =>
    {
        await using var context = new ApplicationDbContext(options);
        await context.Recipes.AddRangeAsync(chunk);
        await context.SaveChangesAsync();
        Console.WriteLine("Inserted one chunk of Recipe");
    }));