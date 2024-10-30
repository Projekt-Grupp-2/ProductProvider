using System;
using System.Collections.Generic;

namespace ProjectProvider.Infrastructure.Entities;

public partial class ColorEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? HexadecimalColor { get; set; }

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
