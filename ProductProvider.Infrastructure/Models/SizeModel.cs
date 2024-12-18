﻿using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class SizeModel
{
    public string Name { get; set; } = null!;

    public virtual ICollection<WarehouseEntity> Warehouses { get; set; } = new List<WarehouseEntity>();
}
