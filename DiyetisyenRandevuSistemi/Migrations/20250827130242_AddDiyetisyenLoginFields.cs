using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyetisyenRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddDiyetisyenLoginFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Diyetisyenler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Diyetisyenler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "Username" },
                values: new object[] { "12345", "ayse" });

            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Password", "Username" },
                values: new object[] { "12345", "mehmet" });

            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Password", "Username" },
                values: new object[] { "12345", "selin" });

            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Password", "Username" },
                values: new object[] { "12345", "baris" });

            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Password", "Username" },
                values: new object[] { "12345", "elif" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Diyetisyenler");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Diyetisyenler");
        }
    }
}
