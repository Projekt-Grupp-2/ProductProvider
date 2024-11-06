using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductProvider.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changednulladevalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3214EC0704288B94", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Hexadecimal_color = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Colors__3214EC07FEE6344D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sizes__3214EC07C28320CC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Short_description = table.Column<string>(type: "text", nullable: false),
                    Long_description = table.Column<string>(type: "text", nullable: true),
                    Category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsTopseller = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__3214EC0768EC9206", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Products__Catego__3552E9B6",
                        column: x => x.Category_id,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Image_URL = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Images__3214EC07066BB8C4", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Images__Product___4589517F",
                        column: x => x.Product_id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discount_price = table.Column<decimal>(type: "money", nullable: true),
                    Start_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    End_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prices__3214EC076581382F", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Prices__Product___42ACE4D4",
                        column: x => x.Product_id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__3214EC07A291AC4A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Reviews__Product__3B0BC30C",
                        column: x => x.Product_id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Unique_product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Size_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Current_stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Warehous__3D606B390D51F906", x => x.Unique_product_id);
                    table.ForeignKey(
                        name: "FK__Warehouse__Color__3EDC53F0",
                        column: x => x.Color_id,
                        principalTable: "Colors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Warehouse__Produ__3DE82FB7",
                        column: x => x.Product_id,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Warehouse__Size___3FD07829",
                        column: x => x.Size_id,
                        principalTable: "Sizes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_Product_id",
                table: "Images",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_Product_id",
                table: "Prices",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category_id",
                table: "Products",
                column: "Category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Product_id",
                table: "Reviews",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Color_id",
                table: "Warehouse",
                column: "Color_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Product_id",
                table: "Warehouse",
                column: "Product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Size_id",
                table: "Warehouse",
                column: "Size_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
