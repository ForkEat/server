using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts
{
    public class ProductStockResponse
    {


        public GetProductResponse Product { get; set; }
        public Unit Unit { get; set; }
        public double Quantity { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        
        public ProductStockResponse()
        {
        }

        public ProductStockResponse(Stock stock)
        {
            Product = new GetProductResponse(stock.Product);
            Unit = stock.Unit;
            Quantity = stock.Quantity;
            PurchaseDate = stock.PurchaseDate;
            BestBeforeDate = stock.BestBeforeDate;
        }
    }
}