namespace ProductProvider.Infrastructure.Entities;

public partial class ReviewEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public int Stars { get; set; }

    public string Text { get; set; } = null!;

    public virtual ProductEntity? Product { get; set; }
}
