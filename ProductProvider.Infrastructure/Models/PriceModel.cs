using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class PriceModel
{
    public Guid? ProductId { get; set; }

    public decimal? Price1 { get; set; }

    public decimal? Discount { get; set; }

    public decimal? DiscountPrice { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ProductEntity? Product { get; set; }
}
