namespace ProductProvider.Infrastructure.Models;

public  class ReviewUpdateRequest
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public int Stars { get; set; }

    public string Text { get; set; } = null!;
}