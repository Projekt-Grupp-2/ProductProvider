using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class SizeService
{
    private readonly IDbContextFactory<DataContext> _contextFactory;
    public SizeService(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }


    /// <summary>
    /// Creates a new size if it doesn't already exist.
    /// </summary>
    /// <param name="sizeModel">The model containing the size information.</param>
    /// <returns>Returns the created SizeEntity. Returns null if the size already exists or if the model is null</returns>
    public async Task<SizeEntity?> CreateSize(SizeModel sizeModel)
    {
        if(sizeModel == null)
        {
            Console.WriteLine("sizeModel cannot be null");
            return null;
        }

        try
        {
            await using var context = _contextFactory.CreateDbContext();
            
            if(!await context.Sizes.AnyAsync(s => s.Name == sizeModel.Name))
            {
                var sizeEntity = new SizeEntity 
                { 
                    Name = sizeModel.Name
                };
                context.Sizes.Add(sizeEntity);
                await context.SaveChangesAsync();
                Console.WriteLine($"Size '{sizeEntity.Name}' created successfully.");
                return sizeEntity;
            }
            else
            {
                Console.WriteLine("The size already exists");
                return null;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;

        }

    }
}
