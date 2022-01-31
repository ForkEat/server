using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrunoZell.ModelBinding;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Services;
using ForkEat.Web.Adapters.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IStockService stockService;
        private readonly DbFileService dbFileService;

        public ProductsController(IProductService productService, IStockService stockService, DbFileService dbFileService)
        {
            this.productService = productService;
            this.stockService = stockService;
            this.dbFileService = dbFileService;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(
            [ModelBinder(BinderType = typeof(JsonModelBinder))]
            CreateUpdateProductRequest payload,
            IFormFile image
        )
        {
            DbFileResponse dbImage = await dbFileService.InsertFileInDb(image);
            payload.ImageId = dbImage.Id;
            
            return Created("", await productService.CreateProduct(payload));
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
        public async Task<ActionResult<GetProductResponse>> UpdateProduct(
            Guid id,
            [ModelBinder(BinderType = typeof(JsonModelBinder))]
            [FromBody] CreateUpdateProductRequest product,            
            IFormFile image
            )
        {
            try
            {
                GetProductResponse updatedProduct = await productService.UpdateProduct(id, product);
                return updatedProduct;
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductResponse>> GetProductById(Guid id)
        {
            try
            {
                GetProductResponse product = await productService.GetProductById(id);

                return product;
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }
        }

        [HttpGet]
        public async Task<IList<GetProductResponse>> GetAllProducts()
        {
            return await productService.GetAllProducts();
        }

        [HttpPut("{id}/stock")]
        public async Task<ActionResult<StockResponse>> CreateOrUpdateStock(Guid id,
            [FromBody] CreateUpdateStockRequest stock)
        {
            try
            {
                StockResponse updatedStock = await stockService.CreateOrUpdateStock(id, stock);
                return updatedStock;
            }
            catch (ProductNotFoundException)
            {
                return NotFound("Product with id: " + id + " was not found");
            }
            catch (UnitNotFoundException)
            {
                return NotFound("Unit with id: " + stock.UnitId + " was not found");
            }
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<IEnumerable<StockResponse>>> GetStocks(Guid id)
        {
            var result = await stockService.GetStocks(id);
            return !result.Any()
                ? NotFound("There is no stock for product with id: " + id)
                : new ActionResult<IEnumerable<StockResponse>>(result);
        }

        [HttpGet("stock")]
        public async Task<ActionResult<List<ProductStockResponse>>> GetStocks()
        {
            return await stockService.GetCompleteStock();
        }
    }
}