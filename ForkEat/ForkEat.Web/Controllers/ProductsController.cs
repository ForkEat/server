using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IStockService stockService;

        public ProductsController(IProductService productService, IStockService stockService)
        {
            this.productService = productService;
            this.stockService = stockService;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateUpdateProductRequest createUpdateProductRequest)
        {
            return Created("", await productService.CreateProduct(createUpdateProductRequest));
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
        
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(Guid id, [FromBody] CreateUpdateProductRequest product)
        {
            Product updatedProduct = null;
            
            try
            {
                updatedProduct = await productService.UpdateProduct(id, product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }

            return updatedProduct;
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

        [HttpPut("{id}/stock")]
        public async Task<ActionResult<StockResponse>> CreateOrUpdateStock(Guid id, [FromBody] CreateUpdateStockRequest stock)
        {
            StockResponse updatedStock = null;

            try
            {
                updatedStock = await stockService.CreateOrUpdateStock(id, stock);
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }
            catch (UnitNotFoundException)
            {
                return NotFound("Unit with id: " + stock.UnitId + " was not found");
            }

            return updatedStock;
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<IEnumerable<StockResponse>>> GetStocks(Guid id)
        {
            var result = await stockService.GetStocks(id);
            return !result.Any()
                ? NotFound("There is no stock for product with id: " + id)
                : new ActionResult<IEnumerable<StockResponse>>(result);
        }
    }
}