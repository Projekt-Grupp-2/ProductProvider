using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Contexts;
using System.Net;

namespace ProductProvider.Functions;

public class GetNewArrivalsFunction
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public GetNewArrivalsFunction(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [Function("GetNewArrivals")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/newarrivals")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetNewArrivals");
        logger.LogInformation("Fetching products from the past two weeks.");

        try
        {
            await using var context = _contextFactory.CreateDbContext();

            var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            var newArrivals = await context.Products
                .Where(p => p.CreatedAt >= twoWeeksAgo)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.ShortDescription,
                    Price = p.Prices.OrderByDescending(price => price.StartDate).FirstOrDefault().Price,
                    DiscountPrice = p.Prices.OrderByDescending(price => price.StartDate).FirstOrDefault().DiscountPrice,
                    ImageUrl = p.Images.FirstOrDefault().ImageUrl
                })
                .ToListAsync();

            var response = req.CreateResponse(newArrivals.Any() ? HttpStatusCode.OK : HttpStatusCode.NoContent);
            if (newArrivals.Any())
            {
                await response.WriteAsJsonAsync(newArrivals);
            }

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