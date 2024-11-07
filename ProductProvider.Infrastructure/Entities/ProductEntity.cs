namespace ProductProvider.Infrastructure.Entities;

public partial class ProductEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public string? LongDescription { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsTopseller { get; set; }

    public virtual CategoryEntity Category { get; set; } = new CategoryEntity();

    public virtual ICollection<ImageEntity>? Images { get; set; } = new List<ImageEntity>();

    public virtual ICollection<PriceEntity>? Prices { get; set; } = new List<PriceEntity>();

    public virtual ICollection<ReviewEntity>? Reviews { get; set; } = new List<ReviewEntity>();

    public virtual ICollection<WarehouseEntity>? Warehouses { get; set; } = new List<WarehouseEntity>();
}
