using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyetisyenRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddAbonelik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 1,
                column: "ResimUrl",
                value: "/İmages/diyetisyen1.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Diyetisyenler",
                keyColumn: "Id",
                keyValue: 1,
                column: "ResimUrl",
                value: null);
        }
    }
}
