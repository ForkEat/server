using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductRequest createProductRequest)
        {
            return Created("", await productService.CreateProduct(createProductRequest));
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(Guid id)
        {
            try
            {
                await productService.DeleteProduct(id);
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }

            return Ok();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(Guid id)
        {
            Product product = null;
            
            try
            {
                product = await productService.GetProductById(id);
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }

            return product;
        }
        
        [HttpGet]
        public async Task<IList<Product>> GetAllProducts()
        {
            return await productService.GetAllProducts();
        }
    }
}