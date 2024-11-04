using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;
using static System.Net.Mime.MediaTypeNames;

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
            return null;
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
            return null;
        }
    }


    /// <summary>
    /// Retrieves a list of images associated with a specific product ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the product whose images are to be retrieved.</param>
    /// <returns> A list of ImageModel corresponding to the images found; otherwise, null. </returns>
    public async Task<List<ImageModel>?> GetImagesByProductIdAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            Console.WriteLine("You must provide a valid imageId");
            return null;
        }
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var imageEntities = await context.Images
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            if (!imageEntities.Any())
            {
                Console.WriteLine($"No images connected to the product id: {productId} were found.");
                return null;
            }

            var imageModels = imageEntities.Select(imageEntity => new ImageModel
            {
                ImageUrl = imageEntity.ImageUrl,
                ProductId = imageEntity.ProductId
            }).ToList();

            return imageModels;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves all images from the database.
    /// </summary>
    /// <returns>A list of ImageModel representing all images; otherwise, null.</returns>

    public async Task<List<ImageModel>?> GetAllImagesAsync()
    {
        try
        {
            await using var context = _contextFactory.CreateDbContext();
            var imageEntities = await context.Images.ToListAsync();
            var imageModels = imageEntities.Select(imageEntity => new ImageModel
            {
                ImageUrl = imageEntity.ImageUrl,
                ProductId = imageEntity.ProductId
            }).ToList();

            return imageModels;
        }
          catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}
