using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService service;

        public RecipesController(IRecipeService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<ActionResult<GetRecipeWithStepsAndIngredientsResponse>> CreateRecipe([FromBody] CreateRecipeRequest request)
        {
            try
            {
                var recipe = await service.CreateRecipe(request);
                return Created("", recipe);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpGet]
        public async Task<ActionResult<IList<GetRecipesResponse>>> GetRecipes([FromQuery] Guid[] ingredients)
        {
            var userId = Guid.Parse(User.Identity.Name);
            var recipes = await (ingredients.Length == 0
                ? this.service.GetRecipes(userId)
                : this.service.SearchRecipeByIngredients(ingredients.ToList()));
            
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
        public async Task<ActionResult<GetRecipeWithStepsAndIngredientsResponse>> UpdateRecipe(Guid recipeId,[FromBody] UpdateRecipeRequest request)
        {
            GetRecipeWithStepsAndIngredientsResponse recipe = await this.service.UpdateRecipe(recipeId,request);
            return recipe;
        }

        [HttpDelete("{recipeId:guid}")]
        public async Task<ActionResult> DeleteRecipeById(Guid recipeId)
        {
            await this.service.DeleteRecipeById(recipeId);
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
}