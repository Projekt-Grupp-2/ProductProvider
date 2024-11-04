using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class PriceService(IDbContextFactory<DataContext> context)
{
    private readonly IDbContextFactory<DataContext> _context = context;

    public async Task<IEnumerable<PriceModel>> GetAllPricesAsync()
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var priceEntity = await context.Prices.ToListAsync();

            var prices = priceEntity.Select(x => new PriceModel
            {
                ProductId = x.ProductId,
                Price1 = x.Price,
                Discount = x.Discount,
                DiscountPrice = x.DiscountPrice,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            }).ToList();

            return prices;
        }
        catch (Exception ex) 
        {
            Console.Write($"An error occured: {ex.Message}");
            return new List<PriceModel>();
        }
    }

    public async Task<List<PriceModel>> GetPricesByProductIdAsync(Guid productId)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var rawPrices = await context.Prices.Where(p => p.ProductId == productId).ToListAsync();

            var prices = rawPrices.Select(price => new PriceModel
            {
                ProductId = price.ProductId,
                Price1 = price.Price,
                Discount = price.Discount,
                DiscountPrice = price.DiscountPrice,
                StartDate = price.StartDate,
                EndDate = price.EndDate,
                IsActive = price.IsActive,
                Product = price.Product
            }).ToList();

            return prices;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<PriceModel>();
        }
    }

    public async Task<bool> DeletePriceAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var priceEntity = await context.Prices.FirstOrDefaultAsync(i => i.Id == id);
            if (priceEntity == null) return false;

            context.Prices.Remove(priceEntity);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    public async Task<PriceEntity> CreatePriceAsync(PriceModel priceModel)
    {
        try
        {
            if (priceModel.ProductId == Guid.Empty)
            {
                Console.WriteLine("Invalid ProductId.");
                return null!;
            }

            await using var context = _context.CreateDbContext();

            var existingPrice = await context.Prices
                .FirstOrDefaultAsync(x => x.ProductId == priceModel.ProductId
                    && x.StartDate == priceModel.StartDate);

            if (existingPrice == null)
            {
                var priceEntity = new PriceEntity
                {
                    ProductId = priceModel.ProductId,
                    Price = priceModel.Price1,
                    Discount = priceModel.Discount,
                    DiscountPrice = priceModel.DiscountPrice,
                    StartDate = priceModel.StartDate,
                    EndDate = priceModel.EndDate,
                    IsActive = priceModel.IsActive
                };

                await context.Prices.AddAsync(priceEntity);
                await context.SaveChangesAsync();
                return priceEntity;
            }

            Console.WriteLine("Price already exists for this product.");
            return existingPrice;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<PriceEntity> UpdatePriceAsync(PriceModel priceModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var priceEntity = await context.Prices.FirstOrDefaultAsync(x => x.Id == priceModel.Id);
            if (priceEntity == null)
            {
                Console.WriteLine("Price not found.");
                return null!;
            }

            var productExists = await context.Products.AnyAsync(p => p.Id == priceModel.ProductId);
            if (!productExists)
            {
                Console.WriteLine("Invalid ProductId.");
                return null!;
            }

            priceEntity.ProductId = priceModel.ProductId;
            priceEntity.Price = priceModel.Price1;
            priceEntity.Discount = priceModel.Discount;
            priceEntity.DiscountPrice = priceModel.DiscountPrice;
            priceEntity.StartDate = priceModel.StartDate;
            priceEntity.EndDate = priceModel.EndDate;
            priceEntity.IsActive = priceModel.IsActive;
            priceEntity.Product = priceModel.Product;
            await context.SaveChangesAsync();

            return priceEntity;
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }
}
