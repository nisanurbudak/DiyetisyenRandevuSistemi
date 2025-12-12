using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyetisyenRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddPaketTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Paketler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HekimId = table.Column<int>(type: "int", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    HaftalikGun = table.Column<int>(type: "int", nullable: true),
                    AylikGun = table.Column<int>(type: "int", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paketler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paketler_Diyetisyenler_HekimId",
                        column: x => x.HekimId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Paketler_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paketler_HekimId",
                table: "Paketler",
                column: "HekimId");

            migrationBuilder.CreateIndex(
                name: "IX_Paketler_UserId",
                table: "Paketler",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paketler");

            migrationBuilder.CreateTable(
                name: "Abonelikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HekimId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    AyinGunu = table.Column<int>(type: "int", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: true),
                    Periyot = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abonelikler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Abonelikler_Diyetisyenler_HekimId",
                        column: x => x.HekimId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Abonelikler_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abonelikler_HekimId",
                table: "Abonelikler",
                column: "HekimId");

            migrationBuilder.CreateIndex(
                name: "IX_Abonelikler_UserId",
                table: "Abonelikler",
                column: "UserId");
        }
    }
}
