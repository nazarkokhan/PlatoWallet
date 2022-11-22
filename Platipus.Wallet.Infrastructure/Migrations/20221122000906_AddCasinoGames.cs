using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class AddCasinoGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameServerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LaunchName = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "casino_games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CasinoId = table.Column<string>(type: "text", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_casino_games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_casino_games_casinos_CasinoId",
                        column: x => x.CasinoId,
                        principalTable: "casinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_casino_games_game_GameId",
                        column: x => x.GameId,
                        principalTable: "game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_casino_games_CasinoId",
                table: "casino_games",
                column: "CasinoId");

            migrationBuilder.CreateIndex(
                name: "IX_casino_games_GameId",
                table: "casino_games",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_game_GameServerId",
                table: "game",
                column: "GameServerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_LaunchName",
                table: "game",
                column: "LaunchName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "casino_games");

            migrationBuilder.DropTable(
                name: "game");
        }
    }
}
