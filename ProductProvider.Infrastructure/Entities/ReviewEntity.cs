using System;
using System.Collections.Generic;

namespace ProductProvider.Infrastructure.Entities;

public partial class ReviewEntity
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public int? Stars { get; set; }

    public string? Text { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
