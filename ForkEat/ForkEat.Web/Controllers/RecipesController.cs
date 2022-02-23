using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrunoZell.ModelBinding;
using ForkEat.Core.Contracts;
using ForkEat.Core.Services;
using ForkEat.Web.Adapters.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService service;
    private readonly DbFileService dbFileService;
    
    public RecipesController(IRecipeService service, DbFileService dbFileService)
    {
        this.service = service;
        this.dbFileService = dbFileService;
    }

    [HttpPost]
    public async Task<ActionResult<GetRecipeWithStepsAndIngredientsResponse>> CreateRecipe(
        [ModelBinder(BinderType = typeof(JsonModelBinder))] CreateRecipeRequest payload,
        IFormFile image
        )
    {
        try
        {
            if (image is not null)
            {
                DbFileResponse dbImage = await dbFileService.InsertFileInDb(image);
                payload.ImageId = dbImage.Id;
            }
            
            var recipe = await service.CreateRecipe(payload);
            return Created("", recipe);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
            
    }

    [HttpGet]
    public async Task<ActionResult<IList<GetRecipesResponse>>> GetRecipes([FromQuery] string[] ingredients)
    {
        var userId = Guid.Parse(User.Identity.Name);
        var recipes = await (ingredients.Length == 0
            ? service.GetRecipes(userId)
            : service.SearchRecipeByIngredientsText(ingredients));

        return Ok(recipes);
    }

    [HttpGet("{recipeId:guid}")]
    public async Task<ActionResult<GetRecipeWithStepsAndIngredientsResponse>> GetRecipeById(Guid recipeId)
    {
        var userId = Guid.Parse(User.Identity.Name);
        GetRecipeWithStepsAndIngredientsResponse recipe = await this.service.GetRecipeById(recipeId, userId);
        return recipe;
    }
        
    [HttpPost("{recipeId:guid}/perform")]
    public async Task<ActionResult> PerformRecipe(Guid recipeId)
    {
        await this.service.PerformRecipe(recipeId);
        return Ok();
    }
        
    [HttpPut("{recipeId:guid}")]
    public async Task<ActionResult<GetRecipeWithStepsAndIngredientsResponse>> UpdateRecipe(
            Guid recipeId,
            [ModelBinder(BinderType = typeof(JsonModelBinder))] UpdateRecipeRequest payload,            
            IFormFile image
        )
    {
        var userId = Guid.Parse(User.Identity.Name);
        try
        {
            if (image is not null)
            {
                Guid oldImageId = (await service.GetRecipeById(recipeId, userId)).ImageId;
                await dbFileService.DeleteFile(oldImageId);
                DbFileResponse newImage = await dbFileService.InsertFileInDb(image);
                payload.ImageId = newImage.Id;
            }

            
            GetRecipeWithStepsAndIngredientsResponse recipe = await this.service.UpdateRecipe(recipeId, payload);
            return recipe;
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{recipeId:guid}")]
    public async Task<ActionResult> DeleteRecipeById(Guid recipeId)
    {
        try
        {
            var recipe = await service.GetRecipeById(recipeId, Guid.Parse(User.Identity.Name));
            await dbFileService.DeleteFile(recipe.ImageId);
            await this.service.DeleteRecipeById(recipeId);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{recipeId:guid}/like")]
    public async Task<ActionResult<bool>> LikeRecipe(Guid recipeId)
    {
        var userId = Guid.Parse(User.Identity.Name);
        var isLiked = await service.LikeRecipe(userId, recipeId);
        return Created("", isLiked);
    }

    [HttpDelete("{recipeId:guid}/like")]
    public async Task<ActionResult> UnlikeRecipe(Guid recipeId)
    {
        var userId = Guid.Parse(User.Identity.Name);
        await service.UnlikeRecipe(userId, recipeId);
        return NoContent();
    }
}