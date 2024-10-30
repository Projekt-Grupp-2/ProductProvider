using ProjectProvider.Infrastructure.Entities;

namespace ProjectProvider.Infrastructure.Models;

public class SizeModel
{
    public string? Name { get; set; }

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
