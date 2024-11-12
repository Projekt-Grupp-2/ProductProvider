using Moq;
using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Models;
using ProductProvider.Infrastructure.Services;
using ProductProvider.Infrastructure.Contexts;
using System.Runtime.CompilerServices;
using ProductProvider.Infrastructure.Entities;

namespace Infrastructure.Tests.Services;
public class ReviewServiceTests
{

    private readonly CategoryEntity _categoryEntity;
    private readonly ProductEntity _productEntity;

    public ReviewServiceTests()
    {
        _categoryEntity = new CategoryEntity
        {
            Name = "Test category",
            Icon = ""
        };

        _productEntity = new ProductEntity
        {
            Name = "Test Product",
            ShortDescription = "Test Short Description",
            LongDescription = "Test Long Description",
            CategoryId = _categoryEntity.Id,
            CreatedAt = DateTime.UtcNow,
            IsTopseller = true,
            Category = _categoryEntity,
            Images = new List<ImageEntity>
            {
                new ImageEntity { ImageUrl = "http://image1.com" },
                new ImageEntity { ImageUrl = "http://image2.com" }
            },
            Prices = new List<PriceEntity>
            {
                new PriceEntity { Price = 100m, Discount = 10m, DiscountPrice = 90m, StartDate = DateTime.UtcNow, IsActive = true }
            },
            Warehouses = new List<WarehouseEntity>
            {
                new WarehouseEntity { CurrentStock = 50 }
            }
        };
    }

    [Fact]
    public async Task CreateReviewAsync_ValidReviewModel_ReturnsReviewEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        var reviewModel = new ReviewModel
        {
            Text = "Great product!",
            Stars = 5,
            ProductId = _productEntity.Id,
        };

        // Act
        var result = await reviewService.CreateReviewAsync(reviewModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewModel.Text, result.Text);
        Assert.Equal(reviewModel.Stars, result.Stars);
        Assert.Equal(reviewModel.ProductId, result.ProductId);

        // Kontrollera om den sparades i databasen
        await using (var context = new DataContext(options))
        {
            var savedReview = await context.Reviews.FindAsync(result.Id);
            Assert.NotNull(savedReview);
            Assert.Equal(reviewModel.Text, savedReview.Text);
            Assert.Equal(reviewModel.Stars, savedReview.Stars);
            Assert.Equal(reviewModel.ProductId, savedReview.ProductId);
        }
    }

    [Fact]
    public async Task CreateReviewAsync_InvalidReviewModel_ReturnsNull()
    {
        //Arrange
        var invalidReviewModel = new ReviewModel
        {
            Text = "Bad review",
            Stars = 1
        };

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new DataContext(options);

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));
        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        //Act
        var result = await reviewService.CreateReviewAsync(invalidReviewModel);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllReviewsAsync_ShouldReturnAllReviews_AsReviewEntities()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new DataContext(options);

        var review1 = context.Reviews.Add(new ReviewEntity
        {
            ProductId = _productEntity.Id,
            Stars = 5,
            Text = "Fantastic product!",
            Product = _productEntity,
        });

        var review2 = context.Reviews.Add(new ReviewEntity
        {
            ProductId = _productEntity.Id,
            Stars = 1,
            Text = "Not my type!",
            Product = _productEntity,
        });

        await context.SaveChangesAsync();


        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        //Act
        var result = await reviewService.GetAllReviewsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.NotNull(review1);
        Assert.NotNull(review2);
    }

    [Fact]
    public async Task GetAllReviewsAsyncShould_ReturnEmptyList_IfNoEntities()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new DataContext(options);

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        //Act
        var result = await reviewService.GetAllReviewsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetReviewsByProductIdShould_ReturnListOfReviews()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(() => new DataContext(options));


        context.Reviews.AddRange(
            new ReviewEntity { ProductId = _productEntity.Id, Stars = 5, Text = "Fantastic product!", Product = _productEntity },
            new ReviewEntity { ProductId = _productEntity.Id, Stars = 1, Text = "Not my type!", Product = _productEntity },
            new ReviewEntity { ProductId = _productEntity.Id, Stars = 3, Text = "Pretty good!", Product = _productEntity }
        );

        await context.SaveChangesAsync();

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        // Act & Assert
        using (var verificationContext = dbContextFactoryMock.Object.CreateDbContext())
        {
            var productEntities = verificationContext.Products.ToList();
            foreach (var product in productEntities)
            {
                var result = await reviewService.GetReviewsByProductId(product.Id);

                Assert.NotNull(result);
                Assert.All(result, r => Assert.Equal(product.Id, r.ProductId));
            }
        }
    }

    [Fact]
    public async Task GetReviewsByProductIdShould_ReturnEmptyListOfReviews_IfProductIdNotExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(() => new DataContext(options));


        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        // Act & Assert
        using (var verificationContext = dbContextFactoryMock.Object.CreateDbContext())
        {

            // Hämta recensioner för varje produkt och kontrollera resultatet
            var result = await reviewService.GetReviewsByProductId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.Empty(result);

        }
    }

    [Fact]
    public async Task GetReviewByIdShould_ReturnOneReviewModel()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new DataContext(options);

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(() => new DataContext(options));

        context.Reviews.AddRange(
                    new ReviewEntity { ProductId = _productEntity.Id, Stars = 5, Text = "Fantastic product!", Product = _productEntity },
                    new ReviewEntity { ProductId = _productEntity.Id, Stars = 1, Text = "Not my type!", Product = _productEntity },
                    new ReviewEntity { ProductId = _productEntity.Id, Stars = 3, Text = "Pretty good!" , Product = _productEntity }
                );

        await context.SaveChangesAsync();

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        var reviewEntities = await reviewService.GetAllReviewsAsync();
        var reviewIds = new List<Guid>();

        foreach (var review in reviewEntities)
        {
            reviewIds.Add(review.Id);
        }

        //Act
        var result = await reviewService.GetReviewById(reviewIds.First());

        //Assert
        Assert.NotNull(result);
        Assert.Equal(reviewIds.First(), result.Id);
        Assert.Equal(5, result.Stars);
    }

    [Fact]
    public async Task GetReviewByIdShould_ReturnNull_IfIdNotExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new DataContext(options);

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(() => new DataContext(options));

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        //Act
        var result = await reviewService.GetReviewById(Guid.NewGuid());

        //Assert
        Assert.Null(result);

    }

    [Fact]
    public async Task UpdateReview_ShouldUpdateReviewEntity_AndReturnReviewModel()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;


        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(() => new DataContext(options));

        var reviewService = new ReviewService(dbContextFactoryMock.Object);

        ReviewEntity reviewEntity;

        await using (var context = new DataContext(options))
        {
            reviewEntity = new ReviewEntity
            {
                ProductId = _productEntity.Id,
                Text = "Old text",
                Stars = 2,
                Product = _productEntity,  
            };
            context.Reviews.Add(reviewEntity);
            await context.SaveChangesAsync();
        }
        var reviewUpdateRequest = new ReviewUpdateRequest
        {
            Id = reviewEntity.Id,
            Stars = 5,
            Text = "Changed",
        };

        //Act
        var result = await reviewService.UpdateReviewAsync(reviewUpdateRequest);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(reviewUpdateRequest.Text, result.Text);
        Assert.Equal(reviewUpdateRequest.Stars, result.Stars);
        Assert.NotEqual(reviewEntity.Stars, result.Stars);
        Assert.NotEqual(reviewEntity.Text, result.Text);

    }

    [Fact]
    public async Task UpdateReview_ShouldReturnNull_IfReviewIdNotExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;


        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(() => new DataContext(options));

        var reviewService = new ReviewService(dbContextFactoryMock.Object);
       
        ReviewEntity reviewEntity;

        await using (var context = new DataContext(options))
        {
            reviewEntity = new ReviewEntity
            {
                ProductId = _productEntity.Id,
                Text = "Old text",
                Stars = 2,
                Product = _productEntity
            };
            context.Reviews.Add(reviewEntity);
            await context.SaveChangesAsync();
        }
        var reviewUpdateRequest = new ReviewUpdateRequest
        {
            Stars = 5,
            Text = "Changed",
        };

        //Act
        var result = await reviewService.UpdateReviewAsync(reviewUpdateRequest);

        //Assert
        Assert.Null(result);


    }


}
