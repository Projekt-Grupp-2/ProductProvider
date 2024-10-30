using System;
using System.Collections.Generic;

namespace ProductProvider.Infrastructure.Entities;

public partial class CategoryEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Icon { get; set; }

    public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}
