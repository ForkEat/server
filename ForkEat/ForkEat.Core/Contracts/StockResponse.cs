using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class StockResponse
{


    public Guid Id { get; set; }
    public double Quantity { get; set; }
    public UnitResponse Unit { get; set; }

    public StockResponse()
    {
    }

    public StockResponse(Stock stock)
    {
        Id = stock.Id;
        Quantity = stock.Quantity;
        Unit = new UnitResponse()
        {
            Name = stock.Unit.Name,
            Symbol = stock.Unit.Symbol,
        };
    }
}