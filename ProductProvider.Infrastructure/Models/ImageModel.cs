using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ImageModel
{
    public Guid? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
