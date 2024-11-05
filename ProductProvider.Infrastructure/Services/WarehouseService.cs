using Microsoft.EntityFrameworkCore;
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


    /// <summary>
    /// Retrieves all unique product variants associated of a product with a specified product ID.
    /// Unique product variants are determined by their associated sizes and colors.
    /// <param name="productId">The identifier of the product for which to retrieve variants.</param>
    /// <returns>Returns an enumerable list of <see cref="WarehouseEntity"/> objects representing the unique product variants. 
    /// Returns an empty list if no variants are found or if an invalid product ID is provided.</returns>
    public async Task<IEnumerable<WarehouseEntity>> GetUniqueProductVariantsByProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            Console.WriteLine("A valid product Id must be provided.");
            return new List<WarehouseEntity>();
        }

        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var warehouseEntities = await context.Warehouses
                .Where(w => w.ProductId == productId)
                .ToListAsync();

            if (!warehouseEntities.Any())
            {
                Console.WriteLine($"No product with the Id: {productId} was found.");
                return new List<WarehouseEntity>();
            }

            return warehouseEntities;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all unique warehouse products from the database.
    /// <returns>An IEnumerable of WarehouseModel objects representing all 
    /// unique products in the warehouse.</returns>
    public async Task<IEnumerable<WarehouseModel>> GetAllUniqueProducts()
    {
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var warehouseModels = await context.Warehouses
                .Select(w => new WarehouseModel
                {
                    ProductId = w.ProductId,
                    ColorId = w.ColorId,
                    SizeId = w.SizeId,
                    Product = w.Product,
                    Color = w.Color,
                    Size = w.Size,
                    CurrentStock = w.CurrentStock,
                }).ToListAsync();



            return warehouseModels;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates a unique warehouse product entry in the database.
    /// This method modifies the properties of an existing warehouse product identified by the provided warehouseId.
    /// 
    /// It checks if the provided WarehouseModel contains new color and size information. If the color or size does not exist
    /// in the database, it creates new entries for them. If they do exist, it updates the product with the corresponding
    /// IDs. The method also updates the current stock of the product. 
    /// 
    /// Returns the updated WarehouseModel if successful, or null if the warehouse model is null, the warehouseId is empty, 
    /// or the product was not found in the database.
    /// 
    /// <param name="newWarehouseModel">The new WarehouseModel containing the updated product details.</param>
    /// <param name="warehouseId">The unique identifier for the warehouse product to be updated.</param>
    /// <returns>The updated WarehouseModel or null.</returns>
    public async Task<WarehouseModel?> UpdateUniqueProductAsync(WarehouseModel newWarehouseModel, Guid warehouseId)
    {
        if (newWarehouseModel == null || warehouseId == Guid.Empty)
        {
            Console.WriteLine("A valid WarehouseModel and a valid warehouseId must be provided.");
            return null;
        }
        try 
        {
            await using var context = _contextFactory.CreateDbContext();
            var warehouseEntity = await context.Warehouses.FirstOrDefaultAsync(w => w.UniqueProductId == warehouseId);

            if(warehouseEntity == null)
            {
                Console.WriteLine($"The product with the id: {warehouseId} was not found");
                return null;
            }

            if (warehouseEntity.ColorId == Guid.Empty) 
            {
                if (newWarehouseModel.Color == null)
                {
                    warehouseEntity.ColorId = newWarehouseModel.ColorId;
                }
                else if (newWarehouseModel.Color.Name != null || newWarehouseModel.Color.HexadecimalColor != null)
                {
                    var newColor = new ColorEntity
                    {
                        Name = newWarehouseModel.Color.Name,
                        HexadecimalColor = newWarehouseModel.Color.HexadecimalColor
                    };

                    await context.Colors.AddAsync(newColor).ConfigureAwait(false);

                    warehouseEntity.ColorId = newColor.Id;
                }
            }
            else
            {
                warehouseEntity.ColorId = newWarehouseModel.ColorId;
            }

            if (warehouseEntity.SizeId == Guid.Empty)
            {
                if (newWarehouseModel.Size == null)
                {
                    warehouseEntity.SizeId = newWarehouseModel.SizeId;
                }
                else if (newWarehouseModel.Size.Name != null)
                {
                    var newSize = new SizeEntity
                    {
                        Name = newWarehouseModel.Size.Name,
                    };

                    await context.Sizes.AddAsync(newSize).ConfigureAwait(false);

                    warehouseEntity.SizeId = newSize.Id;
                }
            }
            else
            {
                warehouseEntity.SizeId = newWarehouseModel.SizeId;
            }

            warehouseEntity.CurrentStock = newWarehouseModel.CurrentStock;

            await context.SaveChangesAsync();

            return new WarehouseModel
            {
                ProductId = warehouseEntity.ProductId,
                ColorId = warehouseEntity.ColorId,
                SizeId = warehouseEntity.SizeId,
                CurrentStock = warehouseEntity.CurrentStock
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }



    /// <summary>
    /// Deletes a unique product from the warehouse by its unique product ID.
    /// </summary>
    /// <param name="uniqueProductId">The GUID of the product to be deleted.</param>
    /// <returns> True if the product was successfully deleted; otherwise, false.</returns>
    public async Task<bool> DeleteUniqueProductAsync(Guid uniqueProductId)
    {
        if (uniqueProductId == Guid.Empty)
        {
            Console.WriteLine("A valid product Id must be provided.");
            return false;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var warehouseEntity = await context.Warehouses.FirstOrDefaultAsync(w => w.UniqueProductId == uniqueProductId);
            
            if (warehouseEntity == null) 
            {
                Console.WriteLine($"The product with the ID: {uniqueProductId} was not found.");
                return false;
            }

            context.Warehouses.Remove(warehouseEntity);
            await context.SaveChangesAsync();

            Console.WriteLine($"The product was successfully deleted.");
            return true;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
}

