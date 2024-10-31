using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class ProductService(IDbContextFactory<DataContext> context)
{
    private readonly IDbContextFactory<DataContext> _context = context;

    public async Task<ProductEntity> CreateProductAsync(ProductModel productModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var productEntity = new ProductEntity
            {
                Name = productModel.Name,
                ShortDescription = productModel.ShortDescription,
                LongDescription = productModel.LongDescription,
                CategoryId = productModel.CategoryId,
                CreatedAt = productModel.CreatedAt ?? DateTime.UtcNow,
                IsTopseller = productModel.IsTopseller ?? false,
                Images = productModel.Images!.Select(image => new ImageEntity
                {
                    ImageUrl = image.ImageUrl,
                }).ToList() ?? new List<ImageEntity>(),
                Prices = productModel.Prices!.Select(price => new PriceEntity
                {
                    Price = price.Price1,
                    Discount = price.Discount,
                    DiscountPrice = price.DiscountPrice,
                    StartDate = price.StartDate,
                    EndDate = price.EndDate,
                    IsActive = price.IsActive,
                }).ToList() ?? new List<PriceEntity>(),
                Warehouses = productModel.Warehouses!.Select(warehouse => new WarehouseEntity
                {
                    CurrentStock = warehouse.CurrentStock,
                }).ToList() ?? new List<WarehouseEntity>()
            };

            await context.Products.AddAsync(productEntity);
            await context.SaveChangesAsync();

            return productEntity;
        }

        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<ProductModel>> GetAllProductsAsync()
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var productEntities = await context.Products
                .Include(image  => image.Images)
                .Include(price => price.Prices)
                .Include(category => category.Category)
                .ToListAsync();

            var products = productEntities.Select(x => new ProductModel
            {
                Name = x.Name,
                ShortDescription = x.ShortDescription,
                LongDescription = x.LongDescription,
                CategoryId = x.CategoryId,
                CreatedAt = x.CreatedAt,
                IsTopseller = x.IsTopseller,
                Images = x.Images.Select(image => new ImageModel
                {
                    ImageUrl = image.ImageUrl
                }).ToList(),
                Prices = x.Prices.Select(price => new PriceModel
                {
                Price1 = price.Price,
                Discount = price.Discount,
                DiscountPrice = price.DiscountPrice,
                StartDate = price.StartDate,
                EndDate = price.EndDate,
                IsActive = price.IsActive
                }).ToList(),

            }).ToList();

            return products;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<ProductModel>();
        }
    }

    public async Task<ProductModel> GetOneProductAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var productEntity = await context.Products
                .Include(x => x.Images)
                .Include(x => x.Prices)
                .Include(x => x.Warehouses)
                .Include(x => x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (productEntity != null )
            {
                var productModel = new ProductModel
                {
                    Name = productEntity.Name,
                    ShortDescription = productEntity.ShortDescription,
                    LongDescription = productEntity.LongDescription,
                    IsTopseller = productEntity.IsTopseller,
                    CreatedAt = productEntity.CreatedAt,
                    Images = productEntity.Images.Select(image => new ImageModel
                    {
                        ImageUrl = image.ImageUrl
                    }).ToList(),
                    Prices = productEntity.Prices.Select(price => new PriceModel
                    {
                        Price1 = price.Price,
                        Discount = price.Discount,
                        DiscountPrice = price.DiscountPrice,
                        StartDate = price.StartDate,
                        EndDate = price.EndDate,
                        IsActive = price.IsActive
                    }).ToList(),
                    Warehouses = productEntity.Warehouses.Select(warehouse => new WarehouseModel
                    {
                        ColorId = warehouse.ColorId,
                        SizeId = warehouse.SizeId,
                        CurrentStock = warehouse.CurrentStock,
                    }).ToList(),
                };

                return productModel;
            }

            return null!;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<ProductEntity> UpdateProductAsync(ProductModel productModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var productEntity = await context.Products
                .Include(p => p.Images)
                .Include(p => p.Prices)
                .Include(p => p.Warehouses)
                .FirstOrDefaultAsync(x => x.Name == productModel.Name);

            if (productEntity != null) 
            {
                productEntity.Name = productModel.Name;
                productEntity.ShortDescription = productModel.ShortDescription;
                productEntity.LongDescription = productModel.LongDescription;
                productEntity.CreatedAt = productModel.CreatedAt;
                productEntity.IsTopseller = productModel.IsTopseller;
                productEntity.CategoryId = productModel.CategoryId;
                productEntity.Images.Clear();
                productEntity.Images = productModel.Images.Select(image => new ImageEntity
                {
                    ImageUrl = image.ImageUrl,
                }).ToList();
                productEntity.Prices.Clear();
                productEntity.Prices.Select(price => new PriceEntity
                {
                    Price = price.Price,
                    Discount = price.Discount,
                    DiscountPrice = price.DiscountPrice,
                    StartDate = price.StartDate,
                    EndDate = price.EndDate,
                    IsActive = price.IsActive
                }).ToList();
                productEntity.Warehouses.Clear();
                productEntity.Warehouses.Select(warehouse => new WarehouseEntity
                {
                    CurrentStock = warehouse.CurrentStock,
                }).ToList();

                await context.SaveChangesAsync();
               
                return productEntity;
            }
            return null!;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var productEntity = await context.Products
                .Include(p => p.Images)
                .Include(p => p.Prices)
                .Include(p => p.Warehouses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (productEntity != null) 
            {
                context.Images.RemoveRange(productEntity.Images);
                context.Prices.RemoveRange(productEntity.Prices);
                context.Warehouses.RemoveRange(productEntity.Warehouses);
                context.Products.Remove(productEntity);

                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
}
