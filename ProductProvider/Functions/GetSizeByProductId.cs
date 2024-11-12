using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;

namespace ProductProvider.Functions
{
    public class GetSizeByProductId
    {
        private readonly ILogger<GetSizeByProductId> _logger;
        private readonly WarehouseService _warehouseService;
        private readonly SizeService _sizeService;

        public GetSizeByProductId(ILogger<GetSizeByProductId> logger, SizeService sizeService, WarehouseService warehouseService)
        {
            _logger = logger;
            _warehouseService = warehouseService;
            _sizeService = sizeService;
        }

        [Function("GetSizeByProductId")]
        public async Task<HttpResponseData> GetSizesByProductId(
                    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ProductSizes/{productId}")] HttpRequestData req,
                    string productId,
                    FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetUniqueProduct");

            try
            {

                if (!Guid.TryParse(productId, out Guid parsedProductId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {productId}");
                    return badRequestResponse;
                }
                var products = await _warehouseService.GetUniqueProductVariantsByProductId(parsedProductId);

                if (!products.Any())
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                var uniqueSizes = new HashSet<SizeEntity>();

                foreach (WarehouseEntity product in products)
                {
                    if (product.SizeId.HasValue)
                    {
                        uniqueSizes.Add(await _sizeService.GetOneSizeAsync(product.SizeId.Value));
                    }
                }

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(uniqueSizes.ToList());


                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
