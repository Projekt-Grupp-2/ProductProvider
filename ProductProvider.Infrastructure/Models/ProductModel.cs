using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ProductModel
{
    public string? Name { get; set; }

    public string? ShortDescription { get; set; }

    public string? LongDescription { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsTopseller { get; set; }

    public virtual CategoryEntity? Category { get; set; }

    public virtual List<ImageModel> Images { get; set; } = new List<ImageModel>();

    public virtual List<PriceModel> Prices { get; set; } = new List<PriceModel>();

    public virtual ICollection<ReviewEntity>? Reviews { get; set; } = new List<ReviewEntity>();

    public virtual List<WarehouseModel> Warehouses { get; set; } = new List<WarehouseModel>();
}
