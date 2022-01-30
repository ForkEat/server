using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ForkEat.Web.Database.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly ApplicationDbContext dbContext;

    public UnitRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Unit> InsertUnit(Unit unit)
    {
        await dbContext.Units.AddAsync(unit);
        await dbContext.SaveChangesAsync();
        return unit;
    }

    public Task<Unit> FindUnitById(Guid id)
    {
        return dbContext
            .Units
            .FirstOrDefaultAsync(unit => unit.Id == id);
    }

    public Task<List<Unit>> FindAllUnits()
    {
        return dbContext.Units.ToListAsync();
    }

    public async Task DeleteUnit(Unit unit)
    {
        dbContext.Units.Remove(unit);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Unit> UpdateUnit(Unit newUnit)
    {
        dbContext.Units.Update(newUnit);
        await dbContext.SaveChangesAsync();
        return await FindUnitById(newUnit.Id);
    }

    public Task<Dictionary<Guid, Unit>> FindUnitsByIds(List<Guid> unitIds)
    {
        return this.dbContext
            .Units
            .Where(unit => unitIds.Contains(unit.Id))
            .ToDictionaryAsync(u => u.Id, u => u);
    }
}