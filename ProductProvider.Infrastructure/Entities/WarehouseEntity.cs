using System;
using System.Collections.Generic;

namespace ProductProvider.Infrastructure.Entities;

public partial class WarehouseEntity
{
    public Guid UniqueProductId { get; set; } = Guid.NewGuid();

    public Guid? ProductId { get; set; }

    public Guid ColorId { get; set; }

    public Guid? SizeId { get; set; }

    public int? CurrentStock { get; set; }

    public virtual ColorEntity? Color { get; set; }

    public virtual ProductEntity? Product { get; set; }

    public virtual SizeEntity? Size { get; set; }
}
