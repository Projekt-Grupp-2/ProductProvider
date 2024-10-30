using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class CategoryModel
{
    public string? Name { get; set; }

    public string? Icon { get; set; }

    public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}
