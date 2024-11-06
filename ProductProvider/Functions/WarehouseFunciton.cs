using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;

namespace ProductProvider.Functions
{
    public class WarehouseFunctions
    {
        private readonly WarehouseService _warehouseService;

        public WarehouseFunctions(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // POST: api/warehouse/create
        [Function("CreateUniqueProduct")]
        public async Task<HttpResponseData> CreateUniqueProduct(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "warehouse/create")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CreateUniqueProduct");

            try
            {
                var uniqueProduct = await req.ReadFromJsonAsync<WarehouseModel>();

                if (uniqueProduct == null)
                {
                    logger.LogError("Product data is missing or invalid.");
                    return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                }

                var createdProduct = await _warehouseService.CreateUniqueProduct(uniqueProduct);

                if (createdProduct == null)
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                }

                return req.CreateResponse(System.Net.HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        // GET: api/warehouse/{uniqueProductId}
        [Function("GetUniqueProduct")]
        public async Task<HttpResponseData> GetUniqueProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "warehouse/{uniqueProductId}")] HttpRequestData req,
            string uniqueProductId,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetUniqueProduct");

            try
            {

                if (!Guid.TryParse(uniqueProductId, out Guid parsedUniqueProductId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {uniqueProductId}");
                    return badRequestResponse;
                }
                var product = await _warehouseService.GetUniqueProduct(parsedUniqueProductId);

                if (product == null)
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(product);


                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        // GET: api/warehouse/variants/{productId}
        [Function("GetUniqueProductVariantsByProductId")]
        public async Task<HttpResponseData> GetUniqueProductVariantsByProductId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "warehouse/variants/{productId}")] HttpRequestData req,
            string productId,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetUniqueProductVariantsByProductId");

            try
            {
                if (!Guid.TryParse(productId, out Guid parsedProductId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {productId}");
                    return badRequestResponse;
                }

                var variants = await _warehouseService.GetUniqueProductVariantsByProductId(parsedProductId);

                if (!variants.Any())
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(variants);


                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        // GET: api/warehouse/all
        [Function("GetAllUniqueProducts")]
        public async Task<HttpResponseData> GetAllUniqueProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "warehouse/all")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetAllUniqueProducts");

            try
            {
                var allProducts = await _warehouseService.GetAllUniqueProducts();

                if (!allProducts.Any())
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NoContent);
                }
                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(allProducts);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        // PUT: api/warehouse/update/{warehouseId}
        [Function("UpdateUniqueProduct")]
        public async Task<HttpResponseData> UpdateUniqueProduct(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "warehouse/update/{warehouseId}")] HttpRequestData req,
            string warehouseId,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("UpdateUniqueProduct");

            try
            {

                if (!Guid.TryParse(warehouseId, out Guid parsedWarehouseId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {warehouseId}");
                    return badRequestResponse;
                }

                var newWarehouseModel = await req.ReadFromJsonAsync<WarehouseModel>();

                if (newWarehouseModel == null)
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                }

                var updatedProduct = await _warehouseService.UpdateUniqueProductAsync(newWarehouseModel, parsedWarehouseId);

                if (updatedProduct == null)
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                return req.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        // DELETE: api/warehouse/delete/{uniqueProductId}
        [Function("DeleteUniqueProduct")]
        public async Task<HttpResponseData> DeleteUniqueProduct(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "warehouse/delete/{uniqueProductId}")] HttpRequestData req,
            string uniqueProductId,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("DeleteUniqueProduct");

            try
            {
                if (!Guid.TryParse(uniqueProductId, out Guid parsedUniqueProductId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {uniqueProductId}");
                    return badRequestResponse;
                }

                var result = await _warehouseService.DeleteUniqueProductAsync(parsedUniqueProductId);

                if (!result)
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
