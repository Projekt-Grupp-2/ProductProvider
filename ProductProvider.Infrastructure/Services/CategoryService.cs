using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class CategoryService(IDbContextFactory<DataContext> context)
{
    private readonly IDbContextFactory<DataContext> _context = context;

    public async Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var categoryEntities = await context.Categories.ToListAsync();

            var categories = categoryEntities.Select(x => new CategoryModel
            {
                Name = x.Name,
                Icon = x.Icon
            }).ToList();

            return categories;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        } 
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var categoryEntity = await context.Categories.FirstOrDefaultAsync(i => i.Id == id);
            if (categoryEntity == null) return false;

            context.Categories.Remove(categoryEntity);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
