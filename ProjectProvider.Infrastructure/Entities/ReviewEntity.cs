using System;
using System.Collections.Generic;

namespace ProjectProvider.Infrastructure.Entities;

public partial class ReviewEntity
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public int? Stars { get; set; }

    public string? Text { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
