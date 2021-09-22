using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;

namespace ForkEat.Core.Services
{
    public interface IUnitService
    {
        Task<Unit> CreateUnit(CreateUpdateUnitRequest createUpdateUnitRequest);
        Task<Unit> GetUnitById(Guid id);
        Task<IList<Unit>> GetAllUnits();
        Task DeleteUnit(Guid id);
        Task<Unit> UpdateUnit(Guid id, CreateUpdateUnitRequest createUpdateUnitRequest);
    }
}