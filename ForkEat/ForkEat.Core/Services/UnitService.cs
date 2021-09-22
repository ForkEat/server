using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;

namespace ForkEat.Core.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository unitRepository;

        public UnitService(IUnitRepository unitRepository)
        {
            this.unitRepository = unitRepository;
        }

        public async Task<Unit> CreateUnit(CreateUpdateUnitRequest createUpdateUnitRequest)
        {
            var unit = new Unit()
            {
                Id = Guid.NewGuid(),
                Name = createUpdateUnitRequest.Name,
                Symbol = createUpdateUnitRequest.Symbol
            };

            return await unitRepository.InsertUnit(unit);
        }

        public async Task<Unit> GetUnitById(Guid id)
        {
            var unit = await unitRepository.FindUnitById(id);

            return unit ?? throw new UnitNotFoundException();
        }

        public async Task<IList<Unit>> GetAllUnits()
        {
            return await unitRepository.FindAllUnits();
        }

        public async Task DeleteUnit(Guid id)
        {
            var unit = await GetUnitById(id);
            await unitRepository.DeleteUnit(unit);
        }

        public async Task<Unit> UpdateUnit(Guid id, CreateUpdateUnitRequest updatedUnit)
        {
            var unitFromDb = await GetUnitById(id);
            unitFromDb.Name = updatedUnit.Name ?? unitFromDb.Name;
            unitFromDb.Symbol = updatedUnit.Symbol ?? unitFromDb.Symbol;
            return await unitRepository.UpdateUnit(unitFromDb);
        }
    }
}