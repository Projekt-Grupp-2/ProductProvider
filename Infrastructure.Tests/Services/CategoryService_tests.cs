using Microsoft.EntityFrameworkCore;
using Moq;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Services;


namespace Infrastructure.Tests.Services;

public class CategoryService_tests
{

    [Fact]
    public async Task GetAllCategoriesAsyncShould_GetAllCategories_ReturnCategories()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "GetAllCategoriesDatabase")
            .Options;

        await using (var context = new DataContext(options))
        {
            context.Categories.Add(new CategoryEntity { Name = "Category1", Icon = "Icon1", Products = new List<ProductEntity>() });
            context.Categories.Add(new CategoryEntity { Name = "Category2", Icon = "Icon2", Products = new List<ProductEntity>() });
            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var categoryService = new CategoryService(dbContextFactoryMock.Object);

        // Act
        var result = await categoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Category1");
        Assert.Contains(result, c => c.Name == "Category2");
    }

    [Fact]
    public async Task DeleteCategoryAsyncShould_DeleteACategory_WhenCategoryExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "DeleteCategoryDatabase")
            .Options;

        var categoryId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            context.Categories.Add(new CategoryEntity { Id = categoryId, Name = "Category1", Icon = "Icon1" });
            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var categoryService = new CategoryService(dbContextFactoryMock.Object);

        // Act
        var result = await categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.True(result);

        await using (var context = new DataContext(options))
        {
            var deletedCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            Assert.Null(deletedCategory);
        }
    }

    [Fact]
    public async Task DeleteCategoryAsyncShould_ReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "DeleteCategoryDatabase")
            .Options;

        var nonExistentCategoryId = Guid.NewGuid();

        await using (var context = new DataContext(options))
        {
            context.Categories.Add(new CategoryEntity { Id = Guid.NewGuid(), Name = "Category1", Icon = "Icon1" });
            await context.SaveChangesAsync();
        }

        var dbContextFactoryMock = new Mock<IDbContextFactory<DataContext>>();
        dbContextFactoryMock.Setup(factory => factory.CreateDbContext()).Returns(new DataContext(options));

        var categoryService = new CategoryService(dbContextFactoryMock.Object);

        // Act
        var result = await categoryService.DeleteCategoryAsync(nonExistentCategoryId);

        // Assert
        Assert.False(result);
    }
}
