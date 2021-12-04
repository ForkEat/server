using System;
using ForkEat.Core.Domain;

namespace ForkEat.Web.Database.Entities
{
    public class StockEntity
    {


        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public Unit Unit { get; set; }
        public ProductEntity Product { get; set; }
        public DateOnly BestBeforeDate { get; set; }
        public DateOnly PurchaseDate { get; set; }

        public StockEntity()
        {
        }

        public StockEntity(Stock stock)
        {
            Id = stock.Id;
            Quantity = stock.Quantity;
            Unit = stock.Unit;
            BestBeforeDate = stock.BestBeforeDate;
            PurchaseDate = stock.PurchaseDate;
            
        }
    }
}