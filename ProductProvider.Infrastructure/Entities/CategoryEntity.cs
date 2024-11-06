namespace ProductProvider.Infrastructure.Entities;

public partial class CategoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = "<i class=\"fa-light fa-bag-shopping\"></i>";

    public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}
