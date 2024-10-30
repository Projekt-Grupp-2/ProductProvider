using System;
using System.Collections.Generic;

namespace ProjectProvider.Infrastructure.Entities;

public partial class SizeEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
