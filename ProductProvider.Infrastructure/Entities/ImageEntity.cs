using System;
using System.Collections.Generic;

namespace ProductProvider.Infrastructure.Entities;

public partial class ImageEntity
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
