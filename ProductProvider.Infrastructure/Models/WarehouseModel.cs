using ProjectProvider.Infrastructure.Entities;

namespace ProjectProvider.Infrastructure.Models;

public class WarehouseModel
{
    public Guid? ProductId { get; set; }

    public Guid? ColorId { get; set; }

    public Guid? SizeId { get; set; }

    public int? CurrentStock { get; set; }

    public virtual ColorEntity? Color { get; set; }

    public virtual ProductEntity? Product { get; set; }

    public virtual SizeEntity? Size { get; set; }
}
