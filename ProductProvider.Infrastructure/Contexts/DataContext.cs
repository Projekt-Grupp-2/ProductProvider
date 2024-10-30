using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectProvider.Infrastructure.Entities;

namespace ProjectProvider.Infrastructure.Contexts;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoryEntity> Categories { get; set; }

    public virtual DbSet<ColorEntity> Colors { get; set; }

    public virtual DbSet<ImageEntity> Images { get; set; }

    public virtual DbSet<PriceEntity> Prices { get; set; }

    public virtual DbSet<ProductEntity> Products { get; set; }

    public virtual DbSet<ReviewEntity> Reviews { get; set; }

    public virtual DbSet<SizeEntity> Sizes { get; set; }

    public virtual DbSet<WarehouseEntity> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Connectionstring");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0704288B94");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Icon).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ColorEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Colors__3214EC07FEE6344D");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.HexadecimalColor)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("Hexadecimal_color");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ImageEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3214EC07066BB8C4");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(2048)
                .HasColumnName("Image_URL");
            entity.Property(e => e.ProductId).HasColumnName("Product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Images__Product___4589517F");
        });

        modelBuilder.Entity<PriceEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prices__3214EC076581382F");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPrice)
                .HasColumnType("money")
                .HasColumnName("Discount_price");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_date");
            entity.Property(e => e.Price1)
                .HasColumnType("money")
                .HasColumnName("Price");
            entity.Property(e => e.ProductId).HasColumnName("Product_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_date");

            entity.HasOne(d => d.Product).WithMany(p => p.Prices)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Prices__Product___42ACE4D4");
        });

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC0768EC9206");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CategoryId).HasColumnName("Category_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.LongDescription)
                .HasColumnType("text")
                .HasColumnName("Long_description");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.ShortDescription)
                .HasColumnType("text")
                .HasColumnName("Short_description");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__3552E9B6");
        });

        modelBuilder.Entity<ReviewEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC07A291AC4A");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ProductId).HasColumnName("Product_id");
            entity.Property(e => e.Text).HasColumnType("text");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Reviews__Product__3B0BC30C");
        });

        modelBuilder.Entity<SizeEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sizes__3214EC07C28320CC");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<WarehouseEntity>(entity =>
        {
            entity.HasKey(e => e.UniqueProductId).HasName("PK__Warehous__3D606B390D51F906");

            entity.ToTable("Warehouse");

            entity.Property(e => e.UniqueProductId)
                .ValueGeneratedNever()
                .HasColumnName("Unique_product_id");
            entity.Property(e => e.ColorId).HasColumnName("Color_id");
            entity.Property(e => e.CurrentStock).HasColumnName("Current_stock");
            entity.Property(e => e.ProductId).HasColumnName("Product_id");
            entity.Property(e => e.SizeId).HasColumnName("Size_id");

            entity.HasOne(d => d.Color).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK__Warehouse__Color__3EDC53F0");

            entity.HasOne(d => d.Product).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Warehouse__Produ__3DE82FB7");

            entity.HasOne(d => d.Size).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FK__Warehouse__Size___3FD07829");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
