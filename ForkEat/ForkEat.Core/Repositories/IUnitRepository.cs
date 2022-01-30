using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Repositories;

public interface IUnitRepository
{
    Task<Unit> InsertUnit(Unit unit);
    Task<Unit> FindUnitById(Guid id);
    Task<List<Unit>> FindAllUnits();
    Task DeleteUnit(Unit unit);
    Task<Unit> UpdateUnit(Unit newUnit);
    Task<Dictionary<Guid,Unit>> FindUnitsByIds(List<Guid> unitIds);
}