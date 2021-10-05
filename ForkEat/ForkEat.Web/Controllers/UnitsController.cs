using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForkEat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService unitService;

        public UnitsController(IUnitService unitService)
        {
            this.unitService = unitService;
        }
        
        [HttpPost]
        public async Task<ActionResult<Unit>> CreateUnit([FromBody] CreateUpdateUnitRequest createUpdateUnitRequest)
        {
            return Created("", await unitService.CreateUnit(createUpdateUnitRequest));
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> DeleteUnit(Guid id)
        {
            try
            {
                await unitService.DeleteUnit(id);
            }
            catch (UnitNotFoundException)
            {
                return NotFound("Unit with id: " + id + " was not found");
            }
        
            return Ok();
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> UpdateUnit(Guid id, [FromBody] CreateUpdateUnitRequest unit)
        {
            Unit updatedUnit = null;
            
            try
            {
                updatedUnit = await unitService.UpdateUnit(id, unit);
            }
            catch (UnitNotFoundException)
            {
                return NotFound("Unit with id: " + id + " was not found");
            }
        
            return updatedUnit;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Unit>> GetUnitById(Guid id)
        {
            Unit unit = null;
            
            try
            {
                unit = await unitService.GetUnitById(id);
            }
            catch (UnitNotFoundException)
            {
                return NotFound("Unit with id: " + id + " was not found");
            }
        
            return unit;
        }
        
        [HttpGet]
        public async Task<IList<Unit>> GetAllUnits()
        {
            return await unitService.GetAllUnits();
        }
    }
}