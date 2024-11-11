using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddPooledDbContextFactory<DataContext>(options =>
            options.UseSqlServer(Environment.GetEnvironmentVariable("RikaProductProviderDB")));

        services.AddSingleton<ProductService>();
        services.AddSingleton<WarehouseService>();
        services.AddSingleton<ReviewService>();
    })
    .Build();

host.Run();