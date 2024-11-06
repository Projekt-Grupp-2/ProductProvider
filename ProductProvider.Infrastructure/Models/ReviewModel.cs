using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ReviewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public int Stars { get; set; }

    public string Text { get; set; } = null!;

    public virtual ProductEntity Product { get; set; } = new ProductEntity();
}
