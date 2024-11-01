using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class ColorService(IDbContextFactory<DataContext> context)
{
    private readonly IDbContextFactory<DataContext> _context = context;


    public async Task<ColorEntity> CreateColorAsync(ColorModel colorModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            if (colorModel != null)
            {
                var colorEntity = new ColorEntity
                {
                    HexadecimalColor = colorModel.HexadecimalColor,
                    Name = colorModel.Name,
                };
                context.Colors.Add(colorEntity);
                await context.SaveChangesAsync();
                await context.DisposeAsync();
                Console.WriteLine("Color successfully created");
                return colorEntity;
            }
            return null!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<ColorModel>> GetAllColorsAsync()
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var colorEntities = await context.Colors.Include(x => x.Warehouses).ToListAsync();
            if (colorEntities.Count == 0)
            {
                return Enumerable.Empty<ColorModel>();
            }
            else
            {
                var colors = colorEntities.Select(r => new ColorModel
                {
                    HexadecimalColor = r.HexadecimalColor,
                    Name = r.Name,
                    Warehouses = r.Warehouses,
                }).ToList();

                return colors;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<ColorModel>> GetAllColorsByProductIdAsync(Guid ProductId)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var productList = await context.Warehouses.Where(x => x.ProductId == ProductId).ToListAsync();
            var colorIds = new List<Guid>();

            foreach (var entity in productList)
            {
                colorIds.Add(entity.ColorId);
            };
            if (colorIds.Count > 0)
            {

                var colorsList = await context.Colors
               .Where(x => colorIds.Contains(x.Id))
               .ToListAsync();

                var colorsOfAProduct = colorsList.Select(x => new ColorModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    HexadecimalColor = x.HexadecimalColor,
                    //Warehouses = x.Warehouses, Kan lägga till detta om vi behöver mappa ut stocks relaterat till color?
                }).ToList();

                return colorsOfAProduct;
            }
            else
            {
                return Enumerable.Empty<ColorModel>();
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<ColorModel> GetColorByIdAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var colorEntity = await context.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (colorEntity != null)
            {
                var color = new ColorModel
                {
                    Name = colorEntity.Name,
                    HexadecimalColor = colorEntity.HexadecimalColor,
                };
                return color;
            }
            return null!;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<ColorModel> GetColorByNameAsync(string colorName)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var colorEntity = await context.Colors.FirstOrDefaultAsync(x => x.Name == colorName);
            if (colorEntity != null)
            {
                var color = new ColorModel
                {
                    Name = colorEntity.Name,
                    HexadecimalColor = colorEntity.HexadecimalColor,
                };
                return color;
            }
            return null!;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<bool> DeleteColorAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var colorEntity = await context.Colors.FirstOrDefaultAsync(i => i.Id == id);
            if (colorEntity == null) return false;

            context.Colors.Remove(colorEntity);
            await context.SaveChangesAsync();
            Console.WriteLine("colorEntity successfully removed");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    public async Task<ColorModel> UpdateColorAsync(ColorModel colorModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var colorEntity = await context.Colors.FirstOrDefaultAsync(x => x.Id == colorModel.Id);
            if (colorEntity == null)
            {
                return null!;
            }
            else
            {
                colorEntity.HexadecimalColor = colorModel.HexadecimalColor;
                colorEntity.Name = colorModel.Name;

                await context.SaveChangesAsync();
                await context.DisposeAsync();
                var color = new ColorModel
                {
                    HexadecimalColor = colorModel.HexadecimalColor,
                    Name = colorModel.Name,
                };
                Console.WriteLine("ColorEntity was successfully updated");
                return color;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }

    }
}


