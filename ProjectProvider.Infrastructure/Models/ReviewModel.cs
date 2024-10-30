using ProjectProvider.Infrastructure.Entities;

namespace ProjectProvider.Infrastructure.Models;

public class ReviewModel
{
    public Guid? ProductId { get; set; }

    public int? Stars { get; set; }

    public string? Text { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
