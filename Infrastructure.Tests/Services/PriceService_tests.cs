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

        var price1 = new PriceEntity
        {
            ProductId = Guid.NewGuid(),
            Price = 100m,
            Discount = 10m,
            DiscountPrice = 90m,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            IsActive = true
        };

        var price2 = new PriceEntity
        {
            ProductId = Guid.NewGuid(),
            Price = 200m,
            Discount = 20m,
            DiscountPrice = 180m,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(2),
            IsActive = true
        };

        await using (var context = new DataContext(options))
        {
            context.Prices.Add(price1);
            context.Prices.Add(price2);
            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.GetAllPricesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPricesByProductIdAsync_ShouldReturnPrices_ForGivenProductId()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetPricesByProductIdDatabase")
            .Options;

        var productId = Guid.NewGuid();

        var price1 = new PriceEntity
        {
            ProductId = productId,
            Price = 100m,
            Discount = 10m,
            DiscountPrice = 90m,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            IsActive = true
        };

        var price2 = new PriceEntity
        {
            ProductId = productId,
            Price = 200m,
            Discount = 20m,
            DiscountPrice = 180m,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(2),
            IsActive = true
        };

        var price3 = new PriceEntity
        {
            ProductId = Guid.NewGuid(),
            Price = 300m,
            Discount = 30m,
            DiscountPrice = 270m,
            StartDate = DateTime.UtcNow.AddDays(-3),
            EndDate = DateTime.UtcNow.AddDays(3),
            IsActive = true
        };

        await using (var context = new DataContext(options))
        {
            context.Prices.Add(price1);
            context.Prices.Add(price2);
            context.Prices.Add(price3);
            await context.SaveChangesAsync();

            var allPrices = await context.Prices.ToListAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var priceService = new PriceService(dbContextFactoryMock.Object);

        // Act
        var result = await priceService.GetPricesByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Price1 == 100m && p.Discount == 10m);
        Assert.Contains(result, p => p.Price1 == 200m && p.Discount == 20m);
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
            context.Prices.Add(new PriceEntity
            {
                Id = priceId,
                ProductId = Guid.NewGuid(),
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

        var productId = Guid.NewGuid();
        var priceId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            context.Products.Add(new ProductEntity { Id = productId, Name = "Test Product" });
            context.Prices.Add(new PriceEntity
            {
                Id = priceId,
                ProductId = productId,
                Price = 100m,
                Discount = 10m,
                DiscountPrice = 90m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                IsActive = true
            });
            await context.SaveChangesAsync();
        }

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
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

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
