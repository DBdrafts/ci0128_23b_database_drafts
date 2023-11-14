using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace LoCoMPro.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_Stagging_Prototype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AsociatedWith_Categories_CategoryName",
                table: "AsociatedWith");

            migrationBuilder.DropForeignKey(
                name: "FK_Register_Store_StoreName_CantonName_ProvinciaName",
                table: "Register");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Register",
                table: "Register");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Point>(
                name: "Geolocation",
                table: "User",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Geolocation",
                table: "Store",
                type: "geography",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProvinciaName",
                table: "Register",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CantonName",
                table: "Register",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Geolocation",
                table: "Canton",
                type: "geography",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Register",
                table: "Register",
                columns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinciaName", "SubmitionDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "CategoryName");

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    SubmitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContributorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => new { x.ImageId, x.ContributorId, x.ProductName, x.StoreName, x.CantonName, x.ProvinceName, x.SubmitionDate });
                    table.ForeignKey(
                        name: "FK_Image_Register_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate",
                        columns: x => new { x.ContributorId, x.ProductName, x.StoreName, x.CantonName, x.ProvinceName, x.SubmitionDate },
                        principalTable: "Register",
                        principalColumns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinciaName", "SubmitionDate" });
                    table.ForeignKey(
                        name: "FK_Image_User_ContributorId",
                        column: x => x.ContributorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReporterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContributorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => new { x.ReporterId, x.ContributorId, x.ProductName, x.StoreName, x.SubmitionDate });
                    table.ForeignKey(
                        name: "FK_Report_Register_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate",
                        columns: x => new { x.ContributorId, x.ProductName, x.StoreName, x.CantonName, x.ProvinceName, x.SubmitionDate },
                        principalTable: "Register",
                        principalColumns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinciaName", "SubmitionDate" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_User_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContributorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantonName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => new { x.ReviewerId, x.ContributorId, x.ProductName, x.StoreName, x.SubmitionDate });
                    table.ForeignKey(
                        name: "FK_Review_Register_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate",
                        columns: x => new { x.ContributorId, x.ProductName, x.StoreName, x.CantonName, x.ProvinceName, x.SubmitionDate },
                        principalTable: "Register",
                        principalColumns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinciaName", "SubmitionDate" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate",
                table: "Image",
                columns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinceName", "SubmitionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Report_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate",
                table: "Report",
                columns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinceName", "SubmitionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Review_ContributorId_ProductName_StoreName_CantonName_ProvinceName_SubmitionDate",
                table: "Review",
                columns: new[] { "ContributorId", "ProductName", "StoreName", "CantonName", "ProvinceName", "SubmitionDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_AsociatedWith_Category_CategoryName",
                table: "AsociatedWith",
                column: "CategoryName",
                principalTable: "Category",
                principalColumn: "CategoryName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Register_Store_StoreName_CantonName_ProvinciaName",
                table: "Register",
                columns: new[] { "StoreName", "CantonName", "ProvinciaName" },
                principalTable: "Store",
                principalColumns: new[] { "Name", "CantonName", "ProvinciaName" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AsociatedWith_Category_CategoryName",
                table: "AsociatedWith");

            migrationBuilder.DropForeignKey(
                name: "FK_Register_Store_StoreName_CantonName_ProvinciaName",
                table: "Register");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Register",
                table: "Register");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Geolocation",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Geolocation",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "Geolocation",
                table: "Canton");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "ProvinciaName",
                table: "Register",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CantonName",
                table: "Register",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Register",
                table: "Register",
                columns: new[] { "ContributorId", "ProductName", "StoreName", "SubmitionDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_AsociatedWith_Categories_CategoryName",
                table: "AsociatedWith",
                column: "CategoryName",
                principalTable: "Categories",
                principalColumn: "CategoryName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Register_Store_StoreName_CantonName_ProvinciaName",
                table: "Register",
                columns: new[] { "StoreName", "CantonName", "ProvinciaName" },
                principalTable: "Store",
                principalColumns: new[] { "Name", "CantonName", "ProvinciaName" });
        }
    }
}
