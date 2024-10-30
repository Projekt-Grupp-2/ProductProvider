using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class CategoryService(IDbContextFactory<DataContext> context)
{
    private readonly IDbContextFactory<DataContext> _context = context;

    public async Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
    {
        await using var context = _context.CreateDbContext();
        var categoryEntities = await context.Categories.Include(c => c.Products).ToListAsync();

        var categories = categoryEntities.Select(x => new CategoryModel
        {
            Name = x.Name,
            Icon = x.Icon,
            Products = x.Products
        }).ToList();

        return categories;
    }

    public async Task<CategoryEntity> CreateCategory(CategoryModel categoryModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var categoryEntity = await context.Categories.FirstOrDefaultAsync(x => x.Name == categoryModel.Name);
            if (categoryEntity == null)
            {
                categoryEntity = new CategoryEntity
                {
                    Icon = categoryModel.Icon,
                    Name = categoryModel.Name,
                };
                context.Categories.Add(categoryEntity);
                await context.SaveChangesAsync();
                return categoryEntity;
            }
            return categoryEntity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
