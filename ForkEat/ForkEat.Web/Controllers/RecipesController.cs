using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}