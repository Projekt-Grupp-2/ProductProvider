using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ColorModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string HexadecimalColor { get; set; } = null!;

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
