using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;

namespace ProductProvider.Infrastructure.Services;

public class SizeService
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public SizeService(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
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
}
