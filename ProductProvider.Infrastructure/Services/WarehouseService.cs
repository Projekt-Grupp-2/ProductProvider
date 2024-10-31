﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class WarehouseService
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public WarehouseService(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }


    /// <summary>
    /// Creates a unique warehouse product entry in the database. 
    /// </summary>
    /// <param name="uniqueProduct">The WarehouseModel containing the product details to create.</param>
    /// <returns>Returns the created WarehouseEntity. Returns null if the unique Product already exists or if the model is null</returns>

    public async Task<WarehouseEntity?> CreateUniqueProduct(WarehouseModel uniqueProduct)
    {
        if (uniqueProduct == null)
        {
            Console.WriteLine("uniqueProduct cannot be null");
            return null;
        }

        try
        {
            await using var context = _contextFactory.CreateDbContext();

            if (!await context.Warehouses.AnyAsync(w =>
                w.ProductId == uniqueProduct.ProductId &&
                w.ColorId == uniqueProduct.ColorId &&
                w.SizeId == uniqueProduct.SizeId))
            {
                var wareHouseEntity = new WarehouseEntity
                {
                    ProductId = uniqueProduct.ProductId,
                    CurrentStock = uniqueProduct.CurrentStock
                };

                if (uniqueProduct.ColorId != null)
                {
                    wareHouseEntity.ColorId = uniqueProduct.ColorId;
                }
                else if (uniqueProduct.Color != null)
                {
                    wareHouseEntity.Color = new ColorEntity
                    {
                        Name = uniqueProduct.Color.Name,
                        HexadecimalColor = uniqueProduct.Color.HexadecimalColor
                    };
                    await context.Colors.AddAsync(wareHouseEntity.Color);
                }

                if (uniqueProduct.SizeId != null)
                {
                    wareHouseEntity.SizeId = uniqueProduct.SizeId;
                }
                else if (uniqueProduct.Size != null)
                {
                    wareHouseEntity.Size = new SizeEntity
                    {
                        Name = uniqueProduct.Size.Name
                    };
                    await context.Sizes.AddAsync(wareHouseEntity.Size);
                }


                await context.Warehouses.AddAsync(wareHouseEntity);
                await context.SaveChangesAsync();

                return wareHouseEntity;
            }
            else
            {
                Console.WriteLine("The unique product already exists");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }


    /// <summary>
    /// Retrieves a unique warehouse product entry from the database by its identifier.
    /// </summary>
    /// <param name="uniqueProductId">The unique identifier for the warehouse product to retrieve.</param>
    /// <returns>A <see cref="WarehouseEntity"/> if found; otherwise, null.</returns>
    public async Task<WarehouseEntity?> GetUniqueProduct(Guid uniqueProductId)
    {
        if (uniqueProductId == Guid.Empty)
        {
            Console.WriteLine("A valid product Id must be provided.");
            return null;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var warehouseEntity = await context.Warehouses.FirstOrDefaultAsync(w => w.UniqueProductId == uniqueProductId);

            if (warehouseEntity == null) 
            {
                Console.WriteLine($"The product with the ID: {uniqueProductId} was not found.");
                return null;
            }

            return warehouseEntity;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
}
