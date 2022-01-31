using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers;

[Route("api/product-types")]
[ApiController]
[Authorize]
public class ProductTypesController : ControllerBase
{

    private readonly IProductTypeService productTypeService;

    public ProductTypesController(IProductTypeService productTypeService)
    {
        this.productTypeService = productTypeService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductType>> CreateProductType([FromBody] CreateUpdateProductTypeRequest createUpdateProductTypeRequest)
    {
        return Created("", await productTypeService.CreateProductType(createUpdateProductTypeRequest));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProductType>> DeleteProductType(Guid id)
    {
        try
        {
            await productTypeService.DeleteProductType(id);
        }
        catch (ProductTypeNotFoundException)
        {
            return NotFound("ProductType with id: " + id + " was not found");
        }

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductType>> UpdateProductType(Guid id, [FromBody] CreateUpdateProductTypeRequest productType)
    {
        ProductType updatedProductType;

        try
        {
            updatedProductType = await productTypeService.UpdateProductType(id, productType);
        }
        catch (ProductTypeNotFoundException)
        {
            return NotFound("ProductType with id: " + id + " was not found");
        }

        return updatedProductType;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductType>> GetProductTypeById(Guid id)
    {
        ProductType productType;

        try
        {
            productType = await productTypeService.GetProductTypeById(id);
        }
        catch (ProductTypeNotFoundException)
        {
            return NotFound("ProductType with id: " + id + " was not found");
        }

        return productType;
    }

    [HttpGet]
    public async Task<IList<ProductType>> GetAllProductTypes()
    {
        return await productTypeService.GetAllProductTypes();
    }
}