using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add configuration sources like appsettings.json or environment variables
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Access the configuration from the context
        var configuration = context.Configuration;

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Initialize DbContext with the correct connection string
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<ProductService>();
        services.AddSingleton<WarehouseService>();
      services.AddSingleton<CategoryService>();
      services.AddSingleton<ReviewService>();

    })
    .Build();

host.Run();