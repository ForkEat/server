using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<List<GetRecipesResponse>>> GetRecipes()
        {
            return await this.service.GetRecipes();
        }

        [HttpGet("{recipeId:guid}")]
        public async Task<ActionResult<GetRecipeWithStepsAndIngredientsResponse>> GetRecipeById(Guid recipeId)
        {
            GetRecipeWithStepsAndIngredientsResponse recipe = await this.service.GetRecipeById(recipeId);
            return recipe;
        }

        [HttpDelete("{recipeId:guid}")]
        public async Task<ActionResult> DeleteRecipeById(Guid recipeId)
        {
            await this.service.DeleteRecipeById(recipeId);
            return NoContent();
        }
    }
}