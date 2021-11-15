using System.Text.Json;
using System.Text.Json.Serialization;
using ForkEat.Core.Contracts;

namespace ForkEat.Web.Adapters.Json
{
    [JsonSerializable(typeof(CreateOrUpdateIngredientRequest))]
    [JsonSerializable(typeof(CreateOrUpdateStepRequest))]
    [JsonSerializable(typeof(CreateRecipeRequest))]
    [JsonSerializable(typeof(CreateUpdateProductRequest))]
    [JsonSerializable(typeof(CreateUpdateStockRequest))]
    [JsonSerializable(typeof(CreateUpdateUnitRequest))]
    [JsonSerializable(typeof(GetIngredientResponse))]
    [JsonSerializable(typeof(GetIngredientResponseWithImage))]
    [JsonSerializable(typeof(GetProductResponse))]
    [JsonSerializable(typeof(GetRecipesResponse))]
    [JsonSerializable(typeof(GetRecipeWithStepsAndIngredientsResponse))]
    [JsonSerializable(typeof(GetStepResponse))]
    [JsonSerializable(typeof(LoginUserRequest))]
    [JsonSerializable(typeof(LoginUserResponse))]
    [JsonSerializable(typeof(ProductStockResponse))]
    [JsonSerializable(typeof(RegisterUserRequest))]
    [JsonSerializable(typeof(RegisterUserResponse))]
    [JsonSerializable(typeof(StockResponse))]
    [JsonSerializable(typeof(UnitResponse))]
    [JsonSerializable(typeof(UpdateRecipeRequest))]
    internal partial class JsonContext : JsonSerializerContext
    {
    }
}