using Microsoft.EntityFrameworkCore;
using Moq;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;

namespace Infrastructure.Tests.Services;

public class PriceService_tests
{
    [Fact]
    public async Task GetAllPricesAsyncShould_ReturnAllPrices()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetAllPricesDatabase")
            .Options;

        await using (var context = new DataContext(options))
        {
            var categoryId = Guid.NewGuid();

            var category = new CategoryEntity
            {
                Id = categoryId,
                Name = "Category1"
            };
            context.Categories.Add(category);

            var productId = Guid.NewGuid();
            var product = new ProductEntity
            {
                Name = "Test",
                CategoryId = Guid.NewGuid(),
                Category = category,
                ShortDescription = "Test",
                CreatedAt = DateTime.UtcNow,
                IsTopseller = false,
            };
            context.Products.Add(product);

            context.Prices.Add(new PriceEntity
            {
                ProductId = Guid.NewGuid(),
                Product = product,
                Price = 100m,
                Discount = 10m,
                DiscountPrice = 90m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                IsActive = true
            });

          
            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.GetAllPricesAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPricesByProductIdAsync_ShouldReturnPrices_ForGivenProductId()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetPricesByProductIdDatabase")
            .Options;

        var categoryId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var category = new CategoryEntity
        {
            Id = categoryId,
            Name = "Category1"
        };

        var product = new ProductEntity
        {
            Id = productId,
            Name = "Test Product",
            CategoryId = categoryId,
            Category = category,
            ShortDescription = "Test",
            CreatedAt = DateTime.UtcNow,
            IsTopseller = false
        };

        var price1 = new PriceEntity
        {
            ProductId = productId,
            Product = product,
            Price = 100m,
            Discount = 10m,
            DiscountPrice = 90m,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            IsActive = true
        };

        var context = new DataContext(options);
        context.Categories.Add(category);
        context.Products.Add(product);
        context.Prices.Add(price1);
        await context.SaveChangesAsync();

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(context);

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.GetPricesByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, p => p.Price1 == 100m && p.Discount == 10m);
    }

    [Fact]
    public async Task DeletePriceAsyncShould_ReturnTrue_WhenPriceExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "DeletePriceDatabase")
            .Options;

        var priceId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            var categoryId = Guid.NewGuid();

            var category = new CategoryEntity
            {
                Id = categoryId,
                Name = "Category1"
            };
            context.Categories.Add(category);

            var productId = Guid.NewGuid();
            var product = new ProductEntity
            {
                Name = "Test",
                CategoryId = Guid.NewGuid(),
                Category = category,
                ShortDescription = "Test",
                CreatedAt = DateTime.UtcNow,
                IsTopseller = false,
            };
            context.Products.Add(product);

            context.Prices.Add(new PriceEntity
            {
                Id = priceId,
                ProductId = Guid.NewGuid(),
                Product = product,
                Price = 100m,
                Discount = 10m,
                DiscountPrice = 90m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                IsActive = true
            });
            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.DeletePriceAsync(priceId);

        // Assert
        Assert.True(result);

        await using (var context = new DataContext(options))
        {
            var deletedPrice = await context.Prices.FindAsync(priceId);
            Assert.Null(deletedPrice);
        }
    }

    [Fact]
    public async Task DeletePriceAsyncShould_ReturnFalse_WhenPriceDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "DeletePriceDatabase")
            .Options;

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.DeletePriceAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreatePriceAsyncShould_ReturnPriceEntity_WhenPriceIsCreated()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "CreatePriceDatabase")
            .Options;

        var productId = Guid.NewGuid();
        var priceModel = new PriceModel
        {
            ProductId = productId,
            Price1 = 100m,
            Discount = 10m,
            DiscountPrice = 90m,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            IsActive = true
        };

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.CreatePriceAsync(priceModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(100m, result.Price);
        Assert.Equal(10m, result.Discount);
        Assert.Equal(90m, result.DiscountPrice);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UpdatePriceAsyncShould_ReturnUpdatedPriceEntity_WhenPriceExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UpdatePriceDatabase")
            .Options;

        var categoryId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var priceId = Guid.NewGuid();

        var context = new DataContext(options);

        var category = new CategoryEntity
        {
            Id = categoryId,
            Name = "Old Category"
        };
        context.Categories.Add(category);

        var product = new ProductEntity
        {
            Id = productId,
            Name = "Test Product",
            CategoryId = categoryId,
            Category = category,
            ShortDescription = "Test Description",
            CreatedAt = DateTime.UtcNow,
            IsTopseller = false,
        };
        context.Products.Add(product);

        context.Prices.Add(new PriceEntity
        {
            Id = priceId,
            ProductId = productId,
            Product = product,
            Price = 100m,
            Discount = 10m,
            DiscountPrice = 90m,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            IsActive = true
        });
        await context.SaveChangesAsync();

        var updatedPriceModel = new PriceModel
        {
            Id = priceId,
            ProductId = productId,
            Price1 = 120m,
            Discount = 15m,
            DiscountPrice = 105m,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(2),
            IsActive = false
        };

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(context);

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.UpdatePriceAsync(updatedPriceModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(priceId, result.Id);
        Assert.Equal(120m, result.Price);
        Assert.Equal(15m, result.Discount);
        Assert.Equal(105m, result.DiscountPrice);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task UpdatePriceAsync_Should_ReturnNull_WhenPriceDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UpdatePriceDatabase")
            .Options;

        var priceModel = new PriceModel
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Price1 = 100m,
            Discount = 10m,
            DiscountPrice = 90m,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            IsActive = true
        };

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.UpdatePriceAsync(priceModel);

        // Assert
        Assert.Null(result);
    }
}
