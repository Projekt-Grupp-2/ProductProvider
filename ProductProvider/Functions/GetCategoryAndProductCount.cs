using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Services;
using System.Net;

namespace ProductProvider.Functions
{
    public class GetCategoryAndProductCount
    {
        private readonly ILogger<CategoryFunctions> _logger;
        private readonly CategoryService _categoryService;

        public GetCategoryAndProductCount(ILogger<CategoryFunctions> logger, CategoryService categoryService)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Function("GetCategoriesAndProductCount")]
        public async Task<HttpResponseData> GetCategoriesAndProductCount(
       [HttpTrigger(AuthorizationLevel.Function, "get", Route = "categories/productcount")] HttpRequestData req,
       FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetCategoriesAndProductCount");

            try
            {
                var allCategories = await _categoryService.GetAllCategoriesAsync();

                var categoriesWithProductCount = allCategories.Select(category => new
                {
                    Icon = category.Icon,
                    CategoryName = category.Name,
                    ProductCount = category.Products.Count 
                }).ToList();

                if (!categoriesWithProductCount.Any())
                {
                    return req.CreateResponse(HttpStatusCode.NoContent);
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(categoriesWithProductCount);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred while processing your request.");
                return errorResponse;
            }
        }
    }
}
