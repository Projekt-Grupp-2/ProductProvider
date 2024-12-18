﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductProvider.Infrastructure.Contexts;

#nullable disable

namespace ProductProvider.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241106093517_Changed nullable values")]
    partial class Changednullablevalues
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.CategoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id")
                        .HasName("PK__Categori__3214EC0704288B94");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ColorEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("HexadecimalColor")
                        .IsRequired()
                        .HasMaxLength(7)
                        .IsUnicode(false)
                        .HasColumnType("varchar(7)")
                        .HasColumnName("Hexadecimal_color");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id")
                        .HasName("PK__Colors__3214EC07FEE6344D");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ImageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)")
                        .HasColumnName("Image_URL");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Product_id");

                    b.HasKey("Id")
                        .HasName("PK__Images__3214EC07066BB8C4");

                    b.HasIndex("ProductId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.PriceEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("Discount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal?>("DiscountPrice")
                        .HasColumnType("money")
                        .HasColumnName("Discount_price");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime")
                        .HasColumnName("End_date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<decimal>("Price")
                        .HasColumnType("money")
                        .HasColumnName("Price");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Product_id");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime")
                        .HasColumnName("Start_date");

                    b.HasKey("Id")
                        .HasName("PK__Prices__3214EC076581382F");

                    b.HasIndex("ProductId");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Category_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime")
                        .HasColumnName("Created_at");

                    b.Property<bool>("IsTopseller")
                        .HasColumnType("bit");

                    b.Property<string>("LongDescription")
                        .HasColumnType("text")
                        .HasColumnName("Long_description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Short_description");

                    b.HasKey("Id")
                        .HasName("PK__Products__3214EC0768EC9206");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ReviewEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Product_id");

                    b.Property<int>("Stars")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("PK__Reviews__3214EC07A291AC4A");

                    b.HasIndex("ProductId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.SizeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id")
                        .HasName("PK__Sizes__3214EC07C28320CC");

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.WarehouseEntity", b =>
                {
                    b.Property<Guid>("UniqueProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Unique_product_id");

                    b.Property<Guid?>("ColorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Color_id");

                    b.Property<int>("CurrentStock")
                        .HasColumnType("int")
                        .HasColumnName("Current_stock");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Product_id");

                    b.Property<Guid?>("SizeId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Size_id");

                    b.HasKey("UniqueProductId")
                        .HasName("PK__Warehous__3D606B390D51F906");

                    b.HasIndex("ColorId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SizeId");

                    b.ToTable("Warehouse", (string)null);
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ImageEntity", b =>
                {
                    b.HasOne("ProductProvider.Infrastructure.Entities.ProductEntity", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Images__Product___4589517F");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.PriceEntity", b =>
                {
                    b.HasOne("ProductProvider.Infrastructure.Entities.ProductEntity", "Product")
                        .WithMany("Prices")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Prices__Product___42ACE4D4");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ProductEntity", b =>
                {
                    b.HasOne("ProductProvider.Infrastructure.Entities.CategoryEntity", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Products__Catego__3552E9B6");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ReviewEntity", b =>
                {
                    b.HasOne("ProductProvider.Infrastructure.Entities.ProductEntity", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Reviews__Product__3B0BC30C");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.WarehouseEntity", b =>
                {
                    b.HasOne("ProductProvider.Infrastructure.Entities.ColorEntity", "Color")
                        .WithMany("Warehouses")
                        .HasForeignKey("ColorId")
                        .HasConstraintName("FK__Warehouse__Color__3EDC53F0");

                    b.HasOne("ProductProvider.Infrastructure.Entities.ProductEntity", "Product")
                        .WithMany("Warehouses")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Warehouse__Produ__3DE82FB7");

                    b.HasOne("ProductProvider.Infrastructure.Entities.SizeEntity", "Size")
                        .WithMany("Warehouses")
                        .HasForeignKey("SizeId")
                        .HasConstraintName("FK__Warehouse__Size___3FD07829");

                    b.Navigation("Color");

                    b.Navigation("Product");

                    b.Navigation("Size");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.CategoryEntity", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ColorEntity", b =>
                {
                    b.Navigation("Warehouses");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.ProductEntity", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Prices");

                    b.Navigation("Reviews");

                    b.Navigation("Warehouses");
                });

            modelBuilder.Entity("ProductProvider.Infrastructure.Entities.SizeEntity", b =>
                {
                    b.Navigation("Warehouses");
                });
#pragma warning restore 612, 618
        }
    }
}
