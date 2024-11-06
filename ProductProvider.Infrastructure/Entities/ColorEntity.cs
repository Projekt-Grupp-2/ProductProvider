namespace ProductProvider.Infrastructure.Entities;

public partial class ColorEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string HexadecimalColor { get; set; } = null!;

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
