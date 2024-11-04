using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace Infrastructure.Tests.Services;

public class ProductService_tests
{
    [Fact]
    public async Task CreateProductAsyncShould_CreateProduct_ReturnProductEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "CreateProductDatabase")
            .Options;

        var productModel = new ProductModel
        {
            Name = "Test Product",
            ShortDescription = "Test Short Description",
            LongDescription = "Test Long Description",
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IsTopseller = true,
            Images = new List<ImageModel>
            {
                new ImageModel { ImageUrl = "http://image1.com" },
                new ImageModel { ImageUrl = "http://image2.com" }
            },
            Prices = new List<PriceModel>
            {
                new PriceModel { Price1 = 100m, Discount = 10m, DiscountPrice = 90m, StartDate = DateTime.UtcNow, IsActive = true }
            },
            Warehouses = new List<WarehouseModel>
            {
                new WarehouseModel { CurrentStock = 50 }
            }
        };

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        // Act
        var result = await productService.CreateProductAsync(productModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productModel.Name, result.Name);
        Assert.Equal(productModel.ShortDescription, result.ShortDescription);
        Assert.Equal(productModel.LongDescription, result.LongDescription);
        Assert.Equal(productModel.CategoryId, result.CategoryId);
        Assert.Equal(productModel.IsTopseller, result.IsTopseller);
        Assert.Equal(2, result.Images.Count);
        Assert.Single(result.Prices);
        Assert.Single(result.Warehouses);

        // Verify that the product was actually saved to the in-memory database
        await using (var context = new DataContext(options))
        {
            var savedProduct = await context.Products.Include(p => p.Images).Include(p => p.Prices).Include(p => p.Warehouses)
                                                     .FirstOrDefaultAsync(p => p.Id == result.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal(productModel.Name, savedProduct.Name);
        }
    }

    [Fact]
    public async Task GetAllProductsAsyncShould_GetAllProducts_ReturnProductModels()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetAllProductsDatabase")
            .Options;

        await using (var context = new DataContext(options))
        {
            context.Products.Add(new ProductEntity
            {
                Name = "Product1",
                ShortDescription = "Short description 1",
                LongDescription = "Long description 1",
                CategoryId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsTopseller = true,
                Images = new List<ImageEntity>
                {
                    new ImageEntity { ImageUrl = "http://image1.com" }
                },
                Prices = new List<PriceEntity>
                {
                    new PriceEntity { Price = 100m, Discount = 10m, DiscountPrice = 90m, StartDate = DateTime.UtcNow, IsActive = true }
                }
            });

            context.Products.Add(new ProductEntity
            {
                Name = "Product2",
                ShortDescription = "Short description 2",
                LongDescription = "Long description 2",
                CategoryId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsTopseller = false,
                Images = new List<ImageEntity>
                {
                    new ImageEntity { ImageUrl = "http://image2.com" }
                },
                Prices = new List<PriceEntity>
                {
                    new PriceEntity { Price = 200m, Discount = 20m, DiscountPrice = 180m, StartDate = DateTime.UtcNow, IsActive = false }
                }
            });

            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        // Act
        var result = await productService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var product1 = result.FirstOrDefault(p => p.Name == "Product1");
        var product2 = result.FirstOrDefault(p => p.Name == "Product2");

        Assert.NotNull(product1);
        Assert.NotNull(product2);
    }

    [Fact]
    public async Task GetOneProductAsyncShould_ReturnCorrectProduct_IfProductExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetOneProductDatabase")
            .Options;

        var productId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            context.Products.Add(new ProductEntity
            {
                Id = productId,
                Name = "Product1",
                ShortDescription = "Short description 1",
                LongDescription = "Long description 1",
                IsTopseller = true,
                CreatedAt = DateTime.UtcNow,
                Category = new CategoryEntity { Id = Guid.NewGuid(), Name = "Category1" },
                Images = new List<ImageEntity>
                {
                    new ImageEntity { ImageUrl = "http://image1.com" }
                },
                Prices = new List<PriceEntity>
                {
                    new PriceEntity { Price = 100m, Discount = 10m, DiscountPrice = 90m, StartDate = DateTime.UtcNow, IsActive = true }
                },
                Warehouses = new List<WarehouseEntity>
                {
                    new WarehouseEntity { ColorId = Guid.NewGuid(), SizeId = Guid.NewGuid(), CurrentStock = 50 }
                }
            });

            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        // Act
        var result = await productService.GetOneProductAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Product1", result.Name);
        Assert.True(result.IsTopseller);
        Assert.Single(result.Images);
        Assert.Single(result.Prices);
        Assert.Single(result.Warehouses);
    }

    [Fact]
    public async Task GetOneProductAsync_Should_ReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetOneProductDatabase")
            .Options;

        await using (var context = new DataContext(options)) { }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        // Act
        var result = await productService.GetOneProductAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProductAsyncShould_UpdateProductAndReturnUpdatedEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UpdateProductDatabase")
            .Options;

        var productId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            context.Products.Add(new ProductEntity
            {
                Id = productId,
                Name = "Old Product",
                ShortDescription = "Old Short Description",
                LongDescription = "Old Long Description",
                CategoryId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                IsTopseller = false,
                Images = new List<ImageEntity>
                {
                    new ImageEntity { ImageUrl = "http://oldimage.com" }
                },
                Prices = new List<PriceEntity>
                {
                    new PriceEntity { Price = 100m, Discount = 10m, DiscountPrice = 90m, StartDate = DateTime.UtcNow.AddDays(-5), IsActive = true }
                },
                Warehouses = new List<WarehouseEntity>
                {
                    new WarehouseEntity { CurrentStock = 50, ColorId = Guid.NewGuid(), SizeId = Guid.NewGuid() }
                }
            });

            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        var updatedProductModel = new ProductModel
        {
            Id = productId,
            Name = "Updated Product",
            ShortDescription = "Updated Short Description",
            LongDescription = "Updated Long Description",
            CategoryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IsTopseller = true,
            Images = new List<ImageModel>
            {
                new ImageModel { ImageUrl = "http://newimage.com" }
            },
            Prices = new List<PriceModel>
            {
                new PriceModel { Price1 = 150m, Discount = 20m, DiscountPrice = 130m, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), IsActive = true }
            },
            Warehouses = new List<WarehouseModel>
            {
                new WarehouseModel { CurrentStock = 100, ColorId = Guid.NewGuid(), SizeId = Guid.NewGuid() }
            }
        };

        // Act
        var result = await productService.UpdateProductAsync(updatedProductModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Product", result.Name);
        Assert.Equal("Updated Short Description", result.ShortDescription);
        Assert.Equal("Updated Long Description", result.LongDescription);
        Assert.True(result.IsTopseller);
        Assert.Single(result.Images);
        Assert.Single(result.Prices);
        Assert.Single(result.Warehouses);
    }

    [Fact]
    public async Task UpdateProductAsyncShould_ReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UpdateProductDatabase")
            .Options;

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        var updatedProductModel = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Non-Existent Product"
        };

        // Act
        var result = await productService.UpdateProductAsync(updatedProductModel);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveProductAsyncShould_DeleteProductAndReturnTrue_WhenProductExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "RemoveProductDatabase")
            .Options;

        var productId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            context.Products.Add(new ProductEntity
            {
                Id = productId,
                Name = "Product1",
                ShortDescription = "Short description",
                LongDescription = "Long description",
                Images = new List<ImageEntity>
                {
                    new ImageEntity { ImageUrl = "http://image1.com" }
                },
                Prices = new List<PriceEntity>
                {
                    new PriceEntity { Price = 100m, Discount = 10m, DiscountPrice = 90m, StartDate = DateTime.UtcNow, IsActive = true }
                },
                Warehouses = new List<WarehouseEntity>
                {
                    new WarehouseEntity { ColorId = Guid.NewGuid(), SizeId = Guid.NewGuid(), CurrentStock = 50 }
                }
            });

            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        // Act
        var result = await productService.DeleteProductAsync(productId);

        // Assert
        Assert.True(result);

        await using (var context = new DataContext(options))
        {
            var deletedProduct = await context.Products.FindAsync(productId);
            Assert.Null(deletedProduct);
            Assert.Empty(context.Images);
            Assert.Empty(context.Prices);
            Assert.Empty(context.Warehouses);
        }
    }

    [Fact]
    public async Task DeleteProductAsync_Should_ReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "RemoveProductDatabase")
            .Options;

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var productService = new ProductService(dbContextFactoryMock.Object);

        // Act
        var result = await productService.DeleteProductAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}

  

