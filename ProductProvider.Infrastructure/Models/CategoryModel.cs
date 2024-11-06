using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class CategoryModel
{
    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}
