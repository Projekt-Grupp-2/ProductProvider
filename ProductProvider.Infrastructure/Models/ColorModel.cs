using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ColorModel
{
    public string? Name { get; set; }

    public string? HexadecimalColor { get; set; }

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
