using System;
using System.Collections.Generic;

namespace ProjectProvider.Infrastructure.Entities;

public partial class PriceEntity
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    public decimal? DiscountPrice { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
