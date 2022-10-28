using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class LinkAwardWithRound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AwardRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardId = table.Column<string>(type: "text", nullable: false),
                    RoundId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwardRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AwardRounds_Awards_AwardId",
                        column: x => x.AwardId,
                        principalTable: "Awards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AwardRounds_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AwardRounds_AwardId",
                table: "AwardRounds",
                column: "AwardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AwardRounds_RoundId",
                table: "AwardRounds",
                column: "RoundId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AwardRounds");
        }
    }
}
