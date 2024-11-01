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
        // Arrange
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Use InMemoryDatabase for testing
            .Options;

        // Create a new context for the in-memory database
        await using (var context = new DataContext(options))
        {
            context.Categories.Add(new CategoryEntity { Name = "Category1", Icon = "Icon1", Products = new List<ProductEntity>() });
            context.Categories.Add(new CategoryEntity { Name = "Category2", Icon = "Icon2", Products = new List<ProductEntity>() });
            await context.SaveChangesAsync();
        }

        // Create a mock of the IDbContextFactory
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
    public async Task DeleteCategoryAsyncShould_DeleteACategoryWithCorrectId_ReturnTrue()
    {
        // Arrange

        // Act

        // Assert
    }
}
