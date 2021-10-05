using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class UnitResponse
    {
        public UnitResponse()
        {
        }


        public string Name { get; set; }
        public string Symbol { get; set; }

        public UnitResponse(Unit unit)
        {
            this.Name = unit.Name;
            this.Symbol = unit.Symbol;
        }
    }
}