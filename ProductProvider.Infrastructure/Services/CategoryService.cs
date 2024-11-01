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
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<CategoryModel>();
        } 
    }

    public async Task<Guid?> GetCategoryIdByNameAsync(string categoryName)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var categoryEntity = await context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            return categoryEntity?.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
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
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
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
              await context.DisposeAsync();
              return categoryEntity;
          }
          return categoryEntity;
      }
      catch (Exception ex)
      {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
      }
  }

  public async Task<CategoryEntity> UpdateCategoryAsync(CategoryModel categoryModel)
  {
      try
      {
          await using var context = _context.CreateDbContext();

          var categoryEntity = await context.Categories.FirstOrDefaultAsync(x => x.Name == categoryModel.Name);
          if (categoryEntity == null)
          {
              return null!;
          }
          else
          {
              categoryEntity.Name = categoryModel.Name;
              categoryEntity.Icon = categoryModel.Icon;
              await context.SaveChangesAsync();
              await context.DisposeAsync();
              return categoryEntity;
          }
      }
      catch (Exception ex)
      {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
      }

  }
}
