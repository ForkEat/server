using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Adapters.Json;
using ForkEat.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ForkEat.Web.Tests.Integration
{
    public class ProductsTests : AuthenticatedTests
    {
        public ProductsTests(WebApplicationFactory<Startup> factory) : base(factory, new string[]{"Stocks","Products","Units"})
        {
        }

        [Fact]
        public async Task CreateProduct_withValidParams_Returns201()
        {
            var productName = "carrot";
            
            // Given
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };

            // When
            var response = await client.PostAsJsonAsync("/api/products", createProductRequest);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadAsAsync<GetProductResponse>();
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(productName);
        }
        
        [Fact]
        public async Task GetProductById_WithExistingProduct_Returns200()
        {
            var productName = "carrot";
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            
            // When
            var response = await client.GetAsync("/api/products/" + productId);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<GetProductResponse>();
            result.Id.Should().Be(productId);
            result.Name.Should().Be(productName);
            result.ImageId.Should().NotBe(Guid.Empty);
        }
        
        [Fact]
        public async Task GetProductById_WithNonExistingProduct_Returns404()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot",
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var response = await client.GetAsync("/api/products/" + Guid.NewGuid());
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task GetAllProducts_Returns200()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot",
                ImageId = Guid.NewGuid()
            };
            
            var createProductRequest2 = new CreateUpdateProductRequest()
            {
                Name = "tomato",
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            await client.PostAsJsonAsync("/api/products", createProductRequest2);
            
            // When
            var response = await client.GetAsync("/api/products");

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<IEnumerable<GetProductResponse>>();
            result.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task DeleteProduct_WithExistingProduct_Returns200()
        {
            var productName = "carrot";
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            
            // When
            var response = await client.DeleteAsync("/api/products/" + productId);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task DeleteProduct_WithNonExistingProduct_Returns404()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot",
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var response = await client.DeleteAsync("/api/products/" + Guid.NewGuid());
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task UpdateProduct_WithExistingProduct_Returns200()
        {
            var productName = "carrot";
            var createUpdateProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            
            // When
            var createUpdateProductRequestUpdated = new CreateUpdateProductRequest()
            {
                Name = productName + " updated"
            };
            var response = await client.PutAsJsonAsync("/api/products/" + productId, createUpdateProductRequestUpdated);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadAsAsync<GetProductResponse>();
            getResult.Id.Should().Be(productId);
            getResult.Name.Should().Be(productName + " updated");
        }
        
        [Fact]
        public async Task UpdateProduct_WithNonExistingProduct_Returns404()
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carrot",
                ImageId = Guid.NewGuid()
            };
            
            // Given
            
            await client.PostAsJsonAsync("/api/products", createProductRequest);
            
            // When
            var createUpdateProductRequestUpdated = new CreateUpdateProductRequest()
            {
                Name = "carrot updated"
            };
            var response = await client.PutAsJsonAsync("/api/products/" + Guid.NewGuid(), createUpdateProductRequestUpdated);
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task UpdateStock_WithExistingStock_Returns200()
        {
            var productName = "carrot";
            var createUpdateProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };
            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };
            
            // Given
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest);
            var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
            var productId = createdProductResult.Id;
            var unitId = createdUnitResult.Id;

            var stock = new CreateUpdateStockRequest
            {
                Quantity = 7,
                UnitId = unitId
            };
            var response = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", stock);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<StockResponse>();
            var stockId = result.Id;

            // When
            var updatedStock = new CreateUpdateStockRequest
            {
                Quantity = 5,
                UnitId = unitId
            };
            var updateResponse = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", updatedStock);
            
            // Then
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId + "/stock");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadAsAsync<IEnumerable<StockResponse>>();
            getResult.First().Quantity.Should().Be(5);
        }
        
        [Fact]
        public async Task UpdateStock_WithNonExistingStock_Returns200()
        {
            var productName = "carrot";
            var createUpdateProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };
            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };

            // Given
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest);
            var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
            var productId = createdProductResult.Id;
            var unitId = createdUnitResult.Id;

            // When
            var stock = new CreateUpdateStockRequest
            {
                Quantity = 7,
                UnitId = unitId
            };
            var response = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", stock);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId + "/stock");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadAsAsync<IEnumerable<StockResponse>>();
            getResult.First().Id.Should().NotBe(Guid.Empty);
            getResult.First().Quantity.Should().Be(7);
        }

        [Fact]
        public async Task UpdateStock_With0Quantity_Returns200()
        {
            // Given
            var productName = "carrot";
            var createUpdateProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ImageId = Guid.NewGuid()
            };

            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };

            
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
            var unitId = createdUnitResult.Id;

            // When
            var stock = new CreateUpdateStockRequest
            {
                Quantity = 7,
                UnitId = unitId
            };
            var response = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", stock);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdStockResult = await response.Content.ReadAsAsync<StockResponse>();
            var stockId = createdStockResult.Id;
            var getResponse = await client.GetAsync("/api/products/" + productId + "/stock" );
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            //Then
            var updatedStock = new CreateUpdateStockRequest
            {
                Quantity = 0,
                UnitId = unitId
            };
            var updateResponse = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", updatedStock);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var getUpdateResponse = await client.GetAsync("/api/products/" + productId + "/stock" );
            getUpdateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStocks_Returns200()
        {
            // Given
            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };

            var createUpdateProductRequest = new CreateUpdateProductRequest()
            {
                Name = "carott",
                ImageId = Guid.NewGuid()
            };

            var createUpdateProductRequest2 = new CreateUpdateProductRequest()
            {
                Name = "carott",
                ImageId = Guid.NewGuid()
            };
            
            var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
            var unitId = createdUnitResult.Id;
            
            var createdProductResponse = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            
            var createdProductResponse2 = await client.PostAsJsonAsync("/api/products", createUpdateProductRequest2);
            var createdProductResult2 = await createdProductResponse2.Content.ReadAsAsync<GetProductResponse>();
            var productId2 = createdProductResult2.Id;

            var stock = new CreateUpdateStockRequest
            {
                Quantity = 7,
                UnitId = unitId
            };

            var stock2 = new CreateUpdateStockRequest
            {
                Quantity = 7,
                UnitId = unitId
            };

            var responsePutStock = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", stock);
            responsePutStock.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdStockResult = await responsePutStock.Content.ReadAsAsync<StockResponse>();
            var stockId = createdStockResult.Id;

            var responsePutStock2 = await client.PutAsJsonAsync("/api/products/" + productId2 + "/stock", stock2);
            responsePutStock2.StatusCode.Should().Be(HttpStatusCode.OK);

            // When
            var response = await client.GetAsync("/api/products/" + productId + "/stock");

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<IEnumerable<StockResponse>>();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(stockId);
        }

        [Fact]
        public async Task GetStock_ReturnsCurrentStockOfAllProducts()
        {
            // Given
            var unit = this.dataFactory.CreateUnit("Kilogramme","kg");

            await this.context.Units.AddAsync(unit);
            await this.context.SaveChangesAsync();
            
            var (product1, product2) = await this.dataFactory.CreateAndInsertProducts();

            var stock1 = new StockEntity()
            {
                Id = Guid.NewGuid(), 
                Product = product1, 
                Quantity = 2,
                Unit = unit, 
                PurchaseDate = DateOnly.FromDateTime(DateTime.Today),
                BestBeforeDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4))
            };
            
            var stock2 = new StockEntity()
            {
                Id = Guid.NewGuid(), 
                Product = product2, 
                Quantity = 4,
                Unit = unit, 
                PurchaseDate = DateOnly.FromDateTime(DateTime.Today),
                BestBeforeDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            await this.context.Stocks.AddRangeAsync(stock1, stock2);
            await this.context.SaveChangesAsync();
            
            // When
            var response = await this.client.GetAsync("/api/products/stock");
            
            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<ProductStockResponse> result = await response.Content.ReadAsAsync<List<ProductStockResponse>>(Formatters);

            result.Should().HaveCount(2);

            ProductStockResponse stockProduct1 = result.First(stock => stock.Product.Id == product1.Id);
            ProductStockResponse stockProduct2 = result.First(stock => stock.Product.Id == product2.Id);

            stockProduct1.Product.Name.Should().Be(product1.Name);
            stockProduct1.Product.ImageId.Should().Be(product1.ImageId);
            stockProduct1.Quantity.Should().Be(2);
            stockProduct1.Unit.Id.Should().Be(unit.Id);
            stockProduct1.Unit.Name.Should().Be(unit.Name);
            stockProduct1.Unit.Symbol.Should().Be(unit.Symbol);
            stockProduct1.BestBeforeDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(4)));
            stockProduct1.PurchaseDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));

            stockProduct2.Product.Name.Should().Be(product2.Name);
            stockProduct2.Product.ImageId.Should().Be(product2.ImageId);
            stockProduct2.Quantity.Should().Be(4);
            stockProduct2.Unit.Id.Should().Be(unit.Id);
            stockProduct2.Unit.Name.Should().Be(unit.Name);
            stockProduct2.Unit.Symbol.Should().Be(unit.Symbol);
            stockProduct2.BestBeforeDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
            stockProduct2.PurchaseDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));
        }
    }
}