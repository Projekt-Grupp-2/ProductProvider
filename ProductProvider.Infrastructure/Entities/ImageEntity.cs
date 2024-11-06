namespace ProductProvider.Infrastructure.Entities;

public partial class ImageEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual ProductEntity Product { get; set; } = new ProductEntity();
}
