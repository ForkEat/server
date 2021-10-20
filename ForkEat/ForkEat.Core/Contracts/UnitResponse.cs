using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class UnitResponse
    {
        public UnitResponse()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public UnitResponse(Unit unit)
        {
            this.Id = unit.Id;
            this.Name = unit.Name;
            this.Symbol = unit.Symbol;
        }
    }
}