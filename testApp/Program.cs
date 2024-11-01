using Microsoft.Extensions.DependencyInjection;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;

class Program
{
    public static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContextFactory<DataContext>() // Register your DbContext
            .AddScoped<PriceService>() // Assuming ProductService has the GetPriceByProductIdAsync method
            .BuildServiceProvider();

        var priceService = serviceProvider.GetRequiredService<PriceService>(); // Assuming you have PriceService
        var existingPriceId = Guid.Parse("502ba629-8f8e-4560-92d3-dad813c5c9ab"); // Replace with actual price ID

        var updatedPriceModel = new PriceModel
        {
            Id = existingPriceId, // Ensure to set the ID of the price you want to update
            ProductId = Guid.Parse("3b9411b5-225b-4d6c-898f-2fc3e2b4dff1"), // Make sure this ID exists
            Price1 = 179.99m,
            Discount = 15m,
            DiscountPrice = 164.99m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true
        };

        var updatedPrice = await priceService.UpdatePriceAsync(updatedPriceModel);
        if (updatedPrice != null)
        {
            Console.WriteLine("Price updated successfully!");
        }
        else
        {
            Console.WriteLine("Failed to update price.");
        }
    }
}

