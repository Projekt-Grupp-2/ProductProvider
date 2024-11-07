using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Services;

namespace ProductProvider.Functions;

public class GetProductById(ILogger<GetProductById> logger, ProductService productService)
{
    private readonly ILogger<GetProductById> _logger = logger;
    private readonly ProductService _productService = productService;

    [Function("GetProductById")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getproduct/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation($"Processing a request to get product with ID: {id}");

        if (!Guid.TryParse(id, out var productId))
        {
            _logger.LogError("Invalid GUID format for product ID.");
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid product ID format.");
            return badRequestResponse;
        }

        var product = await _productService.GetOneProductAsync(productId);
        if (product == null)
        {
            _logger.LogError($"Product with ID {id} not found.");
            var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync("Product not found.");
            return notFoundResponse;
        }

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product);

        return response;
    }
}
