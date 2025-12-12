using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyetisyenRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAbonelik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "Abonelikler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Abonelikler_UserId",
                table: "Abonelikler",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Abonelikler_Users_UserId",
                table: "Abonelikler",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abonelikler_Users_UserId",
                table: "Abonelikler");

            migrationBuilder.DropIndex(
                name: "IX_Abonelikler_UserId",
                table: "Abonelikler");

            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Abonelikler");
        }
    }
}
