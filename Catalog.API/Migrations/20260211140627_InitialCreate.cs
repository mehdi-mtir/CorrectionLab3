using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Catalog.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "ImageUrl", "Name", "Price", "StockQuantity", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Ordinateur portable haute performance avec écran 15 pouces, processeur Intel i7, 16GB RAM", "https://example.com/images/dell-xps-15.jpg", "Laptop Dell XPS 15", 1499.99m, 25, null },
                    { 2, "Electronics", new DateTime(2024, 1, 20, 14, 30, 0, 0, DateTimeKind.Utc), "Smartphone Apple dernière génération avec puce A17 Pro, caméra 48MP", "https://example.com/images/iphone-15-pro.jpg", "iPhone 15 Pro", 1199.99m, 50, null },
                    { 3, "Electronics", new DateTime(2024, 2, 1, 9, 15, 0, 0, DateTimeKind.Utc), "Téléviseur QLED 4K 65 pouces avec HDR, Smart TV et son Dolby Atmos", "https://example.com/images/samsung-qled-65.jpg", "Samsung 65\" QLED TV", 1899.99m, 15, null },
                    { 4, "Audio", new DateTime(2024, 2, 5, 11, 45, 0, 0, DateTimeKind.Utc), "Casque audio sans fil avec réduction de bruit active de pointe", "https://example.com/images/sony-wh1000xm5.jpg", "Sony WH-1000XM5", 399.99m, 40, null },
                    { 5, "Electronics", new DateTime(2024, 2, 10, 16, 20, 0, 0, DateTimeKind.Utc), "Tablette Apple Pro avec puce M2, écran Liquid Retina XDR, 256GB", "https://example.com/images/ipad-pro-12.jpg", "iPad Pro 12.9\"", 1099.99m, 30, null },
                    { 6, "Accessories", new DateTime(2024, 2, 12, 13, 10, 0, 0, DateTimeKind.Utc), "Souris sans fil ergonomique pour professionnels avec 8000 DPI", "https://example.com/images/mx-master-3s.jpg", "Logitech MX Master 3S", 99.99m, 75, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
