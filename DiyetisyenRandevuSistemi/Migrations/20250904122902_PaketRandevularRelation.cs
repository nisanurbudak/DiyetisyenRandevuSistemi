using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyetisyenRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class PaketRandevularRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaketId",
                table: "Randevular",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_PaketId",
                table: "Randevular",
                column: "PaketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Paketler_PaketId",
                table: "Randevular",
                column: "PaketId",
                principalTable: "Paketler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Paketler_PaketId",
                table: "Randevular");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_PaketId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "PaketId",
                table: "Randevular");
        }
    }
}
