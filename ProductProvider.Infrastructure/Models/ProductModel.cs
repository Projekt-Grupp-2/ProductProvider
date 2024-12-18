﻿using ProductProvider.Infrastructure.Entities;

namespace ProductProvider.Infrastructure.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public string? LongDescription { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsTopseller { get; set; }

    public virtual CategoryEntity Category { get; set; } = new CategoryEntity();

    public virtual ICollection<ImageModel> Images { get; set; } = new List<ImageModel>();

    public virtual ICollection<PriceModel> Prices { get; set; } = new List<PriceModel>();

    public virtual ICollection<ReviewEntity>? Reviews { get; set; } = new List<ReviewEntity>();

    public virtual ICollection<WarehouseModel>? Warehouses { get; set; } = new List<WarehouseModel>();
}
