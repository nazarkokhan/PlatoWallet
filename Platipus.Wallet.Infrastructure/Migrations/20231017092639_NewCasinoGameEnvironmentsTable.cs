using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewCasinoGameEnvironmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "casino_game_environments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    casino_id = table.Column<string>(type: "text", nullable: false),
                    game_environment_id = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_casino_game_environments", x => x.id);
                    table.ForeignKey(
                        name: "fk_casino_game_environments_casinos_casino_id",
                        column: x => x.casino_id,
                        principalTable: "casinos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_casino_game_environments_game_environment_game_environment_",
                        column: x => x.game_environment_id,
                        principalTable: "game_environments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_casino_game_environments_casino_id",
                table: "casino_game_environments",
                column: "casino_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_game_environments_created_date",
                table: "casino_game_environments",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_casino_game_environments_game_environment_id",
                table: "casino_game_environments",
                column: "game_environment_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_game_environments_last_updated_date",
                table: "casino_game_environments",
                column: "last_updated_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "casino_game_environments");
        }
    }
}
