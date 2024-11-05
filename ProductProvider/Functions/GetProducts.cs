using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductProvider.Infrastructure.Contexts;

namespace ProductProvider.Functions
{
    public class GetProducts(ILogger<GetProducts> logger, DataContext context)
    {
        private readonly ILogger<GetProducts> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetProducts")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("Processing request to get products.");

            var products = await _context.Products
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    LongDescription = p.LongDescription,
                    Category = p.Category,
                    CreatedAt = p.CreatedAt,
                    IsTopseller = p.IsTopseller,
                    Images = p.Images,
                    Prices = p.Prices,
                    Reviews = p.Reviews,
                    Warehouses = p.Warehouses
                }).ToListAsync();

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteAsJsonAsync(products);

            return response;
        }
    }
}
