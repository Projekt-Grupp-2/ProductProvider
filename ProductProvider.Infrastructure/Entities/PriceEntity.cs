using System;
using System.Collections.Generic;

namespace ProductProvider.Infrastructure.Entities;

public partial class PriceEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    public decimal? DiscountPrice { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
