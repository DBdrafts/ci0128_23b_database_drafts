using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoCoMPro.Migrations
{
    /// <inheritdoc />
    public partial class @fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Name");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryName);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AsociatedWith",
                columns: table => new
                {
                    CategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsociatedWith", x => new { x.CategoryName, x.ProductName });
                    table.ForeignKey(
                        name: "FK_AsociatedWith_Categories_CategoryName",
                        column: x => x.CategoryName,
                        principalTable: "Categories",
                        principalColumn: "CategoryName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsociatedWith_Product_ProductName",
                        column: x => x.ProductName,
                        principalTable: "Product",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Canton",
                columns: table => new
                {
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinciaName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canton", x => new { x.CantonName, x.ProvinciaName });
                    table.ForeignKey(
                        name: "FK_Canton_Provincia_ProvinciaName",
                        column: x => x.ProvinciaName,
                        principalTable: "Provincia",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinciaName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => new { x.Name, x.CantonName, x.ProvinciaName });
                    table.ForeignKey(
                        name: "FK_Store_Canton_CantonName_ProvinciaName",
                        columns: x => new { x.CantonName, x.ProvinciaName },
                        principalTable: "Canton",
                        principalColumns: new[] { "CantonName", "ProvinciaName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProvinciaName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_User_Canton_CantonName_ProvinciaName",
                        columns: x => new { x.CantonName, x.ProvinciaName },
                        principalTable: "Canton",
                        principalColumns: new[] { "CantonName", "ProvinciaName" });
                });

            migrationBuilder.CreateTable(
                name: "Sells",
                columns: table => new
                {
                    ProductName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sells", x => new { x.ProductName, x.StoreName, x.CantonName, x.ProvinceName });
                    table.ForeignKey(
                        name: "FK_Sells_Product_ProductName",
                        column: x => x.ProductName,
                        principalTable: "Product",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sells_Store_StoreName_CantonName_ProvinceName",
                        columns: x => new { x.StoreName, x.CantonName, x.ProvinceName },
                        principalTable: "Store",
                        principalColumns: new[] { "Name", "CantonName", "ProvinciaName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Register",
                columns: table => new
                {
                    SubmitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContributorName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProvinciaName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Register", x => new { x.ContributorName, x.ProductName, x.StoreName, x.SubmitionDate });
                    table.ForeignKey(
                        name: "FK_Register_Product_ProductName",
                        column: x => x.ProductName,
                        principalTable: "Product",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Register_Store_StoreName_CantonName_ProvinciaName",
                        columns: x => new { x.StoreName, x.CantonName, x.ProvinciaName },
                        principalTable: "Store",
                        principalColumns: new[] { "Name", "CantonName", "ProvinciaName" });
                    table.ForeignKey(
                        name: "FK_Register_User_ContributorName",
                        column: x => x.ContributorName,
                        principalTable: "User",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsociatedWith_ProductName",
                table: "AsociatedWith",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_Canton_ProvinciaName",
                table: "Canton",
                column: "ProvinciaName");

            migrationBuilder.CreateIndex(
                name: "IX_Register_ProductName",
                table: "Register",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_Register_StoreName_CantonName_ProvinciaName",
                table: "Register",
                columns: new[] { "StoreName", "CantonName", "ProvinciaName" });

            migrationBuilder.CreateIndex(
                name: "IX_Sells_StoreName_CantonName_ProvinceName",
                table: "Sells",
                columns: new[] { "StoreName", "CantonName", "ProvinceName" });

            migrationBuilder.CreateIndex(
                name: "IX_Store_CantonName_ProvinciaName",
                table: "Store",
                columns: new[] { "CantonName", "ProvinciaName" });

            migrationBuilder.CreateIndex(
                name: "IX_User_CantonName_ProvinciaName",
                table: "User",
                columns: new[] { "CantonName", "ProvinciaName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsociatedWith");

            migrationBuilder.DropTable(
                name: "Register");

            migrationBuilder.DropTable(
                name: "Sells");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Canton");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Name");
        }
    }
}
