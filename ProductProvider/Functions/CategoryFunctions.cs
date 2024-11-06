using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;

namespace ProductProvider.Functions
{
    public class CategoryFunctions
    {
        private readonly ILogger<CategoryFunctions> _logger;
        private readonly CategoryService _categoryService;

        public CategoryFunctions(ILogger<CategoryFunctions> logger, CategoryService categoryService)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Function("CategoryFunctions")]
        public async Task<HttpResponseData> CreateCategory([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("CreateCategory");
            
            try
            {
                var category = await req.ReadFromJsonAsync<CategoryModel>();

                if (category == null) 
                {
                    logger.LogError("Category data is missing or invalid.");
                    return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                }

                var createdCategory = await _categoryService.CreateCategory(category);

                if (createdCategory == null)
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


        


        /*
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
        }*/
    }
}
