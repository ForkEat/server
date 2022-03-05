using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace ForkEat.Web.Tests.Integration
{
    public class ProductsTests : AuthenticatedTests
    {
        public ProductsTests(WebApplicationFactory<Startup> factory) : base(factory,
            new string[] {"Stocks", "Products", "Units","Files"})
        {
        }

        [Fact]
        public async Task CreateProduct_withValidParams_Returns201AndInsertsFile()
        {
            // Given
            using var formDataContent = CreateProductRequestContent("carrot");

            // When
            var response = await client.PostAsync("/api/products", formDataContent);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadAsAsync<GetProductResponse>();
            
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("carrot");

            await AssertThatFileIsEquivalentToImageFromApi("TestAssets/test-file.gif", result.ImageId);
        }
        
        [Fact]
        public async Task GetProductById_WithExistingProduct_Returns200()
        {
            using var carrotCreateProductContent = CreateProductRequestContent("carrot");
            var createdProductResponse = await client.PostAsync("/api/products", carrotCreateProductContent);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;

            // When
            var response = await client.GetAsync("/api/products/" + productId);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<GetProductResponse>();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("carrot");
            result.ImageId.Should().NotBe(Guid.Empty);
        }
        
        [Fact]
        public async Task GetProductById_WithExistingProduct_Returns200AndProductType()
        {
            // Given
            var productType = new ProductType() {Id = Guid.NewGuid(), Name = "Test Product Type"};
            await this.context.ProductTypes.AddAsync(productType);
            await this.context.SaveChangesAsync();
            using var carrotCreateProductContent = CreateProductRequestContent("carrot", productType.Id);
            
            var createdProductResponse = await client.PostAsync("/api/products", carrotCreateProductContent);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;

            // When
            var response = await client.GetAsync("/api/products/" + productId);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<GetProductResponse>();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("carrot");
            result.ImageId.Should().NotBe(Guid.Empty);
            result.ProductType.Should().NotBeNull();
            result.ProductType.Id.Should().NotBeEmpty();
            result.ProductType.Name.Should().Be("Test Product Type");
        }

        [Fact]
        public async Task GetProductById_WithNonExistingProduct_Returns404()
        {
            // When
            var response = await client.GetAsync("/api/products/" + Guid.NewGuid());

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllProducts_Returns200AndProductType()
        {
            // Given
            var productType = new ProductType() {Id = Guid.NewGuid(), Name = "Vegetable"};
            await this.context.ProductTypes.AddAsync(productType);
            await this.context.SaveChangesAsync();
            using var carrotCreateProductContent = CreateProductRequestContent("carrot", productType.Id);
            using var tomatoCreateProductContent = CreateProductRequestContent("tomato", productType.Id);
            
            await client.PostAsync("/api/products", carrotCreateProductContent);
            await client.PostAsync("/api/products", tomatoCreateProductContent);
          
            // When
            var response = await client.GetAsync("/api/products");

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<IList<GetProductResponse>>();
            result.Should().HaveCount(2);
            result.First(product => product.Name == "carrot").ProductType.Should().NotBeNull();
            result.First(product => product.Name == "carrot").ProductType.Name.Should().Be("Vegetable");
            
            result.First(product => product.Name == "tomato").ProductType.Should().NotBeNull();
            result.First(product => product.Name == "tomato").ProductType.Name.Should().Be("Vegetable");
        }

        [Fact]
        public async Task DeleteProduct_WithExistingProduct_Returns200()
        {
            // Given
            using var createProductContent = CreateProductRequestContent("carrot");

            var createdProductResponse = await client.PostAsync("/api/products", createProductContent);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;

            // When
            var response = await client.DeleteAsync("/api/products/" + productId);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var getResponse = await client.GetAsync("/api/products/" + productId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
            await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
            (await specialContext.Files.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task DeleteProduct_WithNonExistingProduct_Returns404()
        {
            // When
            var response = await client.DeleteAsync("/api/products/" + Guid.NewGuid());

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateProduct_WithExistingProductAndNewImage_Returns200()
        {

            // Given
            var productName = "carrot";
            var createUpdateProductRequest = CreateProductRequestContent(productName);
            var createdProductResponse = await client.PostAsync("/api/products", createUpdateProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            ProductEntity productEntity = await context.Products.FirstAsync(record => record.Id == productId);
            Guid imageId = productEntity.ImageId;

            // When
            var createUpdateProductRequestUpdated = CreateProductRequestContent($"{productName} updated");
            var response = await client.PutAsync("/api/products/" + productId, createUpdateProductRequestUpdated);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await client.GetAsync("/api/products/" + productId);
            
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadAsAsync<GetProductResponse>();
            getResult.Id.Should().Be(productId);
            getResult.Name.Should().Be($"{productName} updated");

            await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
            ProductEntity productEntityUpdated = await specialContext.Products.FirstAsync(record => record.Id == productId);
            productEntityUpdated.Name.Should().Be($"{productName} updated");
            productEntityUpdated.ImageId.Should().NotBe(imageId);

            int filesCount = await specialContext.Files.CountAsync();
            filesCount.Should().Be(1);
        }

        [Fact]
        public async Task UpdateProduct_WithoutImage_UpdatesDataButDoesntModifyImage()
        {
            // Given
            var productName = "carrot";
            var createUpdateProductRequest = CreateProductRequestContent(productName);
            var createdProductResponse = await client.PostAsync("/api/products", createUpdateProductRequest);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;
            ProductEntity productEntity = await context.Products.FirstAsync(record => record.Id == productId);
            Guid imageId = productEntity.ImageId;

            // When
            var createUpdateProductRequestUpdated = UpdateProductRequestContentNoImage($"{productName} updated");
            var response = await client.PutAsync("/api/products/" + productId, createUpdateProductRequestUpdated);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var specialContext = new ApplicationDbContext(this.GetOptionsForOtherDbContext());
            ProductEntity productEntityUpdated = await specialContext.Products.FirstAsync(record => record.Id == productId);
            productEntityUpdated.Name.Should().Be($"{productName} updated");
            productEntityUpdated.ImageId.Should().Be(imageId);

            int filesCount = await specialContext.Files.CountAsync();
            filesCount.Should().Be(1);
        }



        [Fact]
        public async Task UpdateProduct_WithNonExistingProduct_Returns404()
        {
            // Given
            var createUpdateProductRequestUpdated = UpdateProductRequestContentNoImage("carrot");
            
            // When
            var response =
                await client.PutAsync("/api/products/" + Guid.NewGuid(), createUpdateProductRequestUpdated);

            // Then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateStock_WithExistingStock_Returns200()
        {
            // Given
            using var createCarrotProductContent = CreateProductRequestContent("carrot");
            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };


            var createdProductResponse = await client.PostAsync("/api/products", createCarrotProductContent);
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
            var createUpdateProductRequest = CreateProductRequestContent(productName);
            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };

            // Given

            var createdProductResponse = await client.PostAsync("/api/products", createUpdateProductRequest);
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
            using var createCarrotProductContent = CreateProductRequestContent("carrot");
            var createUpdateUnitRequest = new CreateUpdateUnitRequest()
            {
                Name = "kilogram",
                Symbol = "kg"
            };


            var createdProductResponse = await client.PostAsync("/api/products", createCarrotProductContent);
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
            var getResponse = await client.GetAsync("/api/products/" + productId + "/stock");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            //Then
            var updatedStock = new CreateUpdateStockRequest
            {
                Quantity = 0,
                UnitId = unitId
            };
            var updateResponse = await client.PutAsJsonAsync("/api/products/" + productId + "/stock", updatedStock);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getUpdateResponse = await client.GetAsync("/api/products/" + productId + "/stock");
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

            using var createCarrotProductContent = CreateProductRequestContent("carrot");
            using var createTomatoProductContent = CreateProductRequestContent("tomato");


            var createdUnitResponse = await client.PostAsJsonAsync("/api/units", createUpdateUnitRequest);
            var createdUnitResult = await createdUnitResponse.Content.ReadAsAsync<Unit>();
            var unitId = createdUnitResult.Id;

            var createdProductResponse = await client.PostAsync("/api/products", createCarrotProductContent);
            var createdProductResult = await createdProductResponse.Content.ReadAsAsync<GetProductResponse>();
            var productId = createdProductResult.Id;

            var createdProductResponse2 = await client.PostAsync("/api/products", createTomatoProductContent);
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
            var unit = this.dataFactory.CreateUnit("Kilogramme", "kg");

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
            List<ProductStockResponse> result =
                await response.Content.ReadAsAsync<List<ProductStockResponse>>(Formatters);

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
        
        
        private static MultipartFormDataContent CreateProductRequestContent(string productName, Guid? productTypeId = null)
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName,
                ProductTypeId = productTypeId
            };


            var formDataContent = new MultipartFormDataContent(Guid.NewGuid().ToString());

            var stringContent = new StringContent(JsonConvert.SerializeObject(createProductRequest));
            formDataContent.Add(stringContent, "payload");

            var streamContent = new StreamContent(File.OpenRead("TestAssets/test-file.gif"));
            streamContent.Headers.Add("Content-Type", "application/octet-stream");

            formDataContent.Add(streamContent, "image", Path.GetFileName("TestAssets/test-file.gif"));
            return formDataContent;
        }
        
        private HttpContent UpdateProductRequestContentNoImage(string productName)
        {
            var createProductRequest = new CreateUpdateProductRequest()
            {
                Name = productName
            };


            var formDataContent = new MultipartFormDataContent(Guid.NewGuid().ToString());

            var stringContent = new StringContent(JsonConvert.SerializeObject(createProductRequest));
            formDataContent.Add(stringContent, "payload");
            return formDataContent;
        }
    }
}