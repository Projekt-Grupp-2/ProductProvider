using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;

namespace ProductProvider.Functions;

public class GetProducts(ILogger<GetProducts> logger, ProductService productService)
{
    private readonly ILogger<GetProducts> _logger = logger;
    private readonly ProductService _productService = productService;

    [Function("GetProducts")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProducts")] HttpRequestData req)
    {
        _logger.LogInformation("Processing request to get products.");
        try
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var categoryName = queryParams["categoryName"];

            IEnumerable<ProductModel> products;

            if (string.IsNullOrEmpty(categoryName))
            {
                products = await _productService.GetAllProductsAsync();
            }
            else
            {
                products = await _productService.GetProductsByCategoryAsync(categoryName);
            }

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteAsJsonAsync(products);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when getting products.");

            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Message = "An error occurred", Error = ex.Message });

            return errorResponse;
        }
    }
}
