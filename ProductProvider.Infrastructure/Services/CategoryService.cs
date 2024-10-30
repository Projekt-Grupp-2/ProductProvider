using Microsoft.EntityFrameworkCore;
using ProjectProvider.Infrastructure.Contexts;

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
}
