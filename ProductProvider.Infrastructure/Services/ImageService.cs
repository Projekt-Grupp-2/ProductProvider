using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class ImageService(IDbContextFactory<DataContext> contextFactory)
{
    /*public class ImageModel
{
    public Guid? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
     */

    private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;


    /// <summary>
    /// Creates a new image entity in the database if it does not already exist.
    /// </summary>
    /// <param name="imageModel">The model containing the image data.</param>
    /// <returns> The created ImageEntity, or null if the image already exists or an error occurred.</returns>
    public async Task<ImageEntity?> CreateImageAsync(ImageModel imageModel)
    {
        if (imageModel == null)
        {
            Console.WriteLine("You must provide an ImageModel");
            return null;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();

            if (!await context.Images.AnyAsync(i => i.ImageUrl == imageModel.ImageUrl))
            {
                var imageEntity = new ImageEntity
                {
                    ProductId = imageModel.ProductId,
                    ImageUrl = imageModel.ImageUrl
                };

                await context.Images.AddAsync(imageEntity);
                await context.SaveChangesAsync();
                return imageEntity;
            }
            Console.WriteLine("The image already exists.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }


    /// <summary>
    /// Retrieves an image by its ID.
    /// </summary>
    /// <param name="imageId">The unique identifier of the image to retrieve.</param>
    /// <returns>An ImageModel if an ImageEntity with imageId is found; otherwise, null.</returns>
    public async Task<ImageModel?> GetOneImageByIdAsync(Guid imageId)
    {
        if (imageId == Guid.Empty)
        {
            Console.WriteLine("You must provide a valid imageId");
            return null;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var imageEntity = await context.Images.FirstOrDefaultAsync(i => i.Id == imageId);

            if (imageEntity == null)
            {
                Console.WriteLine($"The image with the id: {imageId} was not found.");
                return null;
            }
            ImageModel imageModel = new ImageModel
            {
                ImageUrl = imageEntity.ImageUrl,
                ProductId = imageEntity.ProductId
            };

            return imageModel;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

}
