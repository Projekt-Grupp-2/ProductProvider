using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Services;

namespace ProductProvider.Functions;

public class GetReviewsByProductId(ILogger<GetReviewsByProductId> logger, ReviewService reviewService)
{
    private readonly ILogger<GetReviewsByProductId> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("GetReviewsByProductId")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "getreviews/{productId}")] HttpRequest req, Guid productId)
    {

        try
        {
            if (productId != Guid.Empty)
            {
                var reviews = await _reviewService.GetReviewsByProductId(productId);

                if (reviews != null)
                {
                    return new OkObjectResult(new { Status = 200, Message = "Reviews returned successfully", Reviews = reviews });
                }
                else
                {
                    return new NotFoundObjectResult(new { Status = 404, Message = "No reviews found for this product." });
                }
            }
            return new BadRequestObjectResult(new { Status = 400, Message = "Invalid productId." });
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occurred: {ex.Message}");
            return new BadRequestObjectResult(new { Status = 400, Message = "Unable to get reviews right now." });
        }
    }

}
