using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;
using System.Threading;

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

        [Function("CreateCategoryFunction")]
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


        [Function("GetOneCategoryIdFunction")]
        public async Task<HttpResponseData> GetCategoryId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "categories/{categoryName}")] HttpRequestData req,
            string categoryName, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetOneCategoryId");

            try
            {
                var categoryId = await _categoryService.GetCategoryIdByNameAsync(categoryName);

                if (categoryId == null)
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }


                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(categoryId);


                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("GetAllCategories")]
        public async Task<HttpResponseData> GetAllCategories(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "categories/all")] HttpRequestData req, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetAllCategories");

            try
            {
                var allCategories = await _categoryService.GetAllCategoriesAsync();

                if(!allCategories.Any())
                {
                    return req.CreateResponse(System.Net.HttpStatusCode.NoContent);
                }

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(allCategories);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("UpdateCategory")]
        public async Task<HttpResponseData> UpdateCategory(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "categories/update")] HttpRequestData req, 
            CategoryModel categoryToUpdate, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("UpdateCategory");

            try
            {

                var updatedCategory = await _categoryService.UpdateCategoryAsync(categoryToUpdate);

                if (updatedCategory == null)
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


        [Function("DeleteCategory")]
        public async Task<HttpResponseData> DeleteCategory(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "categories/delete/{categoryId}")] HttpRequestData req,
            string categoryId, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("DeleteCategory");

            try
            {
                if (!Guid.TryParse(categoryId, out Guid parsedCategoryId))
                {
                    // If parsing fails, return a BadRequest with a helpful message
                    var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync($"Invalid productId format. Expected a valid GUID, but received: {categoryId}");
                    return badRequestResponse;
                }

                var deletedCategory = await _categoryService.DeleteCategoryAsync(parsedCategoryId);

                if(!deletedCategory)
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
    }
}
