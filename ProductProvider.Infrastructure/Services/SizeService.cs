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
    /// Retrieves a size entity by its Id.
    /// </summary>
    /// <param name="sizeId">The Id of the size to retrieve.</param>
    /// <returns>
    /// A <see cref="SizeEntity"/> if found; otherwise, null.</returns>
    public async Task<SizeEntity?> GetOneSizeAsync(Guid sizeId)
    {
        if (sizeId == Guid.Empty)
        {
            Console.WriteLine("A valid size name must be provided.");
            return null;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var sizeEntity = await context.Sizes.FirstOrDefaultAsync(x => x.Id == sizeId);

            if (sizeEntity == null)
            {
                Console.WriteLine($"Size {sizeId} not found.");
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
    /// Deletes a size with the specifiend name if it exists.
    /// </summary>
    /// <param name="sizeName">The name of the size to be deleted</param>
    /// <returns>True if successfully deleted, otherwise returns False</returns>

    public async Task<bool> DeleteSizeAsync(string sizeName)
    {
        if (string.IsNullOrWhiteSpace(sizeName)) 
        {
            Console.WriteLine("A valid size name must be provided.");
            return false; 
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var sizeEntityToRemove = await context.Sizes.FirstOrDefaultAsync(s => s.Name == sizeName);

            if (sizeEntityToRemove == null)
            {
                Console.WriteLine($"No size found with the name: '{sizeName}'.");
                return false;
            }
            else
            {
              context.Sizes.Remove(sizeEntityToRemove);
              await context.SaveChangesAsync();
              Console.WriteLine($"Size '{sizeName}' was successfully removed.");
              return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something went wrong:" + ex.Message);
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
    /// Updates an existing size entity in the database with the properties from the provided <see cref="SizeModel"/>.
    /// </summary>
    /// <param name="newSizeModel">The new size model containing the updated information.</param>
    /// <param name="oldSizeName">The name of the existing size to be updated.</param>
    /// <returns> A <see cref="SizeModel"/> representing the updated size if successful; otherwise, null.</returns>

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
            Console.WriteLine("Something went wrong:" + ex.Message);
            throw;
        }
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
