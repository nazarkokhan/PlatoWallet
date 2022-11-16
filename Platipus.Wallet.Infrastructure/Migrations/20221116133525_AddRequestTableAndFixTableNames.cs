using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class AddRequestTableAndFixTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AwardRounds_Awards_AwardId",
                table: "AwardRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_AwardRounds_Rounds_RoundId",
                table: "AwardRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Users_UserId",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_CasinoCurrencies_Casinos_CasinoId",
                table: "CasinoCurrencies");

            migrationBuilder.DropForeignKey(
                name: "FK_CasinoCurrencies_Currencies_CurrencyId",
                table: "CasinoCurrencies");

            migrationBuilder.DropForeignKey(
                name: "FK_MockedErrors_Users_UserId",
                table: "MockedErrors");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Users_UserId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Rounds_RoundId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Casinos_CasinoId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Currencies_CurrencyId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rounds",
                table: "Rounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Casinos",
                table: "Casinos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Awards",
                table: "Awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MockedErrors",
                table: "MockedErrors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CasinoCurrencies",
                table: "CasinoCurrencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AwardRounds",
                table: "AwardRounds");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "transactions");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "sessions");

            migrationBuilder.RenameTable(
                name: "Rounds",
                newName: "rounds");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "currencies");

            migrationBuilder.RenameTable(
                name: "Casinos",
                newName: "casinos");

            migrationBuilder.RenameTable(
                name: "Awards",
                newName: "awards");

            migrationBuilder.RenameTable(
                name: "MockedErrors",
                newName: "mocked_errors");

            migrationBuilder.RenameTable(
                name: "CasinoCurrencies",
                newName: "casino_currencies");

            migrationBuilder.RenameTable(
                name: "AwardRounds",
                newName: "award_rounds");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserName",
                table: "users",
                newName: "IX_users_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CurrencyId",
                table: "users",
                newName: "IX_users_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CasinoId",
                table: "users",
                newName: "IX_users_CasinoId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_RoundId",
                table: "transactions",
                newName: "IX_transactions_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_UserId",
                table: "sessions",
                newName: "IX_sessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rounds_UserId",
                table: "rounds",
                newName: "IX_rounds_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Currencies_Name",
                table: "currencies",
                newName: "IX_currencies_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_UserId",
                table: "awards",
                newName: "IX_awards_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MockedErrors_UserId",
                table: "mocked_errors",
                newName: "IX_mocked_errors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CasinoCurrencies_CurrencyId",
                table: "casino_currencies",
                newName: "IX_casino_currencies_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_CasinoCurrencies_CasinoId",
                table: "casino_currencies",
                newName: "IX_casino_currencies_CasinoId");

            migrationBuilder.RenameIndex(
                name: "IX_AwardRounds_RoundId",
                table: "award_rounds",
                newName: "IX_award_rounds_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_AwardRounds_AwardId",
                table: "award_rounds",
                newName: "IX_award_rounds_AwardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactions",
                table: "transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sessions",
                table: "sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rounds",
                table: "rounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_currencies",
                table: "currencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_casinos",
                table: "casinos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_awards",
                table: "awards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mocked_errors",
                table: "mocked_errors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_casino_currencies",
                table: "casino_currencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_award_rounds",
                table: "award_rounds",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requests_UserId",
                table: "requests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_award_rounds_awards_AwardId",
                table: "award_rounds",
                column: "AwardId",
                principalTable: "awards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_award_rounds_rounds_RoundId",
                table: "award_rounds",
                column: "RoundId",
                principalTable: "rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_awards_users_UserId",
                table: "awards",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_casino_currencies_casinos_CasinoId",
                table: "casino_currencies",
                column: "CasinoId",
                principalTable: "casinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_casino_currencies_currencies_CurrencyId",
                table: "casino_currencies",
                column: "CurrencyId",
                principalTable: "currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mocked_errors_users_UserId",
                table: "mocked_errors",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rounds_users_UserId",
                table: "rounds",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_rounds_RoundId",
                table: "transactions",
                column: "RoundId",
                principalTable: "rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_casinos_CasinoId",
                table: "users",
                column: "CasinoId",
                principalTable: "casinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_currencies_CurrencyId",
                table: "users",
                column: "CurrencyId",
                principalTable: "currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_award_rounds_awards_AwardId",
                table: "award_rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_award_rounds_rounds_RoundId",
                table: "award_rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_awards_users_UserId",
                table: "awards");

            migrationBuilder.DropForeignKey(
                name: "FK_casino_currencies_casinos_CasinoId",
                table: "casino_currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_casino_currencies_currencies_CurrencyId",
                table: "casino_currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_mocked_errors_users_UserId",
                table: "mocked_errors");

            migrationBuilder.DropForeignKey(
                name: "FK_rounds_users_UserId",
                table: "rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_rounds_RoundId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_users_casinos_CasinoId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_currencies_CurrencyId",
                table: "users");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transactions",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sessions",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rounds",
                table: "rounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_casinos",
                table: "casinos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_awards",
                table: "awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mocked_errors",
                table: "mocked_errors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_casino_currencies",
                table: "casino_currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_award_rounds",
                table: "award_rounds");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "transactions",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "sessions",
                newName: "Sessions");

            migrationBuilder.RenameTable(
                name: "rounds",
                newName: "Rounds");

            migrationBuilder.RenameTable(
                name: "currencies",
                newName: "Currencies");

            migrationBuilder.RenameTable(
                name: "casinos",
                newName: "Casinos");

            migrationBuilder.RenameTable(
                name: "awards",
                newName: "Awards");

            migrationBuilder.RenameTable(
                name: "mocked_errors",
                newName: "MockedErrors");

            migrationBuilder.RenameTable(
                name: "casino_currencies",
                newName: "CasinoCurrencies");

            migrationBuilder.RenameTable(
                name: "award_rounds",
                newName: "AwardRounds");

            migrationBuilder.RenameIndex(
                name: "IX_users_UserName",
                table: "Users",
                newName: "IX_Users_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_users_CurrencyId",
                table: "Users",
                newName: "IX_Users_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_users_CasinoId",
                table: "Users",
                newName: "IX_Users_CasinoId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_RoundId",
                table: "Transactions",
                newName: "IX_Transactions_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_sessions_UserId",
                table: "Sessions",
                newName: "IX_Sessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_rounds_UserId",
                table: "Rounds",
                newName: "IX_Rounds_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_currencies_Name",
                table: "Currencies",
                newName: "IX_Currencies_Name");

            migrationBuilder.RenameIndex(
                name: "IX_awards_UserId",
                table: "Awards",
                newName: "IX_Awards_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_mocked_errors_UserId",
                table: "MockedErrors",
                newName: "IX_MockedErrors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_casino_currencies_CurrencyId",
                table: "CasinoCurrencies",
                newName: "IX_CasinoCurrencies_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_casino_currencies_CasinoId",
                table: "CasinoCurrencies",
                newName: "IX_CasinoCurrencies_CasinoId");

            migrationBuilder.RenameIndex(
                name: "IX_award_rounds_RoundId",
                table: "AwardRounds",
                newName: "IX_AwardRounds_RoundId");

            migrationBuilder.RenameIndex(
                name: "IX_award_rounds_AwardId",
                table: "AwardRounds",
                newName: "IX_AwardRounds_AwardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rounds",
                table: "Rounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Casinos",
                table: "Casinos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Awards",
                table: "Awards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MockedErrors",
                table: "MockedErrors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CasinoCurrencies",
                table: "CasinoCurrencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AwardRounds",
                table: "AwardRounds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AwardRounds_Awards_AwardId",
                table: "AwardRounds",
                column: "AwardId",
                principalTable: "Awards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AwardRounds_Rounds_RoundId",
                table: "AwardRounds",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Users_UserId",
                table: "Awards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CasinoCurrencies_Casinos_CasinoId",
                table: "CasinoCurrencies",
                column: "CasinoId",
                principalTable: "Casinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CasinoCurrencies_Currencies_CurrencyId",
                table: "CasinoCurrencies",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MockedErrors_Users_UserId",
                table: "MockedErrors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Users_UserId",
                table: "Rounds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserId",
                table: "Sessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Rounds_RoundId",
                table: "Transactions",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Casinos_CasinoId",
                table: "Users",
                column: "CasinoId",
                principalTable: "Casinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Currencies_CurrencyId",
                table: "Users",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
