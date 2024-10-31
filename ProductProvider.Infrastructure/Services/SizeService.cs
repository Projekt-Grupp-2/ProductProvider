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
    /// Retrieves a size entity by its name.
    /// </summary>
    /// <param name="sizeName">The name of the size to retrieve.</param>
    /// <returns>
    /// A <see cref="SizeEntity"/> if found; otherwise, null.</returns>
    public async Task<SizeEntity?> GetOneSizeAsync(string sizeName)
    {
        if (string.IsNullOrWhiteSpace(sizeName))
        {
            Console.WriteLine("A valid size name must be provided.");
            return null;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var sizeEntity = await context.Sizes.FirstOrDefaultAsync(x => x.Name == sizeName);

            if (sizeEntity == null)
            {
                Console.WriteLine($"Size {sizeName} not found.");
                return null;
            }

            return sizeEntity;


        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all size entities and maps them to <see cref="SizeModel"/> objects.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="SizeModel"/> representing all sizes in the database. </returns>

    public async Task<IEnumerable<SizeModel>> GetAllSizesAsync()
    {
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var sizeEntities = await context.Sizes.ToListAsync();

            var sizes = sizeEntities.Select(x => new SizeModel
            {
                Name = x.Name

            }).ToList();

            return sizes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }


    /// <summary>
    /// Updates an existing size entity based on the provided <see cref="SizeModel"/>.
    /// </summary>
    /// <param name="sizeModel">The size model containing the updated information.</param>
    /// <returns>A <see cref="SizeModel"/> representing the updated size if successful; otherwise, null. </returns>

    public async Task<SizeModel?> UpdateSizeAsync(SizeModel newSizeModel, string oldSizeName)
    {

        if (newSizeModel == null || string.IsNullOrWhiteSpace(newSizeModel.Name))
        {
            Console.WriteLine("A valid size model with a name must be provided.");
            return null; 
        }

        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var sizeEntity = await context.Sizes.FirstOrDefaultAsync(x => x.Name == oldSizeName);

            if (sizeEntity == null)
            {
                Console.WriteLine($"Size {oldSizeName} not found.");
                return null;
            }

            sizeEntity.Name = newSizeModel.Name;

            await context.SaveChangesAsync();

            return new SizeModel 
            { 
                Name = sizeEntity.Name,
            };

        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
}
