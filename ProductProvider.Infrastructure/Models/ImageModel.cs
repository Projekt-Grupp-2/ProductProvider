using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ImageModel
{
    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual ProductEntity Product { get; set; } = new ProductEntity();
}
