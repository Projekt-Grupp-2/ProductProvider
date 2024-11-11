using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Services;

namespace ProductProvider.Functions
{
    public class GetColorByProductId
    {
        private readonly ILogger<GetColorByProductId> _logger;
        private readonly WarehouseService _warehouseService;
        private readonly ColorService _colorService;

        public GetColorByProductId(ILogger<GetColorByProductId> logger, ColorService colorService)
        {
            _logger = logger;
            _colorService = colorService;
        }

        [Function("GetColorsByProductId")]
        public async Task<HttpResponseData> GetColorsByProductId(
                [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ProductColors/{productId}")] HttpRequestData req,
                string productId,
                FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetColorByProductId");

            try
            {
                if (!Guid.TryParse(productId, out Guid parsedProductId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {productId}");
                    return badRequestResponse;
                }

                var colors = await _colorService.GetAllColorsByProductIdAsync(parsedProductId);

                if (!colors.Any())
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }


                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(colors);


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
