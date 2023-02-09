using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ColumnToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_casino_games_casinos_CasinoId",
                table: "casino_games");

            migrationBuilder.DropForeignKey(
                name: "FK_casino_games_game_GameId",
                table: "casino_games");

            migrationBuilder.DropForeignKey(
                name: "FK_mocked_errors_users_UserId",
                table: "mocked_errors");

            migrationBuilder.DropForeignKey(
                name: "FK_requests_users_UserId",
                table: "requests");

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
                name: "PK_requests",
                table: "requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mocked_errors",
                table: "mocked_errors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game",
                table: "game");

            migrationBuilder.DropPrimaryKey(
                name: "PK_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_casinos",
                table: "casinos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_casino_games",
                table: "casino_games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_casino_currencies",
                table: "casino_currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_awards",
                table: "awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_award_rounds",
                table: "award_rounds");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "users",
                newName: "balance");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "users",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "SwUserId",
                table: "users",
                newName: "sw_user_id");

            migrationBuilder.RenameColumn(
                name: "IsDisabled",
                table: "users",
                newName: "is_disabled");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "users",
                newName: "currency_id");

            migrationBuilder.RenameColumn(
                name: "CasinoId",
                table: "users",
                newName: "casino_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_UserName",
                table: "users",
                newName: "ix_users_user_name");

            migrationBuilder.RenameIndex(
                name: "IX_users_CurrencyId",
                table: "users",
                newName: "ix_users_currency_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_CasinoId",
                table: "users",
                newName: "ix_users_casino_id");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "transactions",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "transactions",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "InternalId",
                table: "transactions",
                newName: "internal_id");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "transactions",
                newName: "created_date");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_RoundId",
                table: "transactions",
                newName: "ix_transactions_round_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "sessions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "sessions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "sessions",
                newName: "expiration_date");

            migrationBuilder.RenameIndex(
                name: "IX_sessions_UserId",
                table: "sessions",
                newName: "ix_sessions_user_id");

            migrationBuilder.RenameColumn(
                name: "Finished",
                table: "rounds",
                newName: "finished");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "rounds",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "rounds",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_rounds_UserId",
                table: "rounds",
                newName: "ix_rounds_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "requests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "requests",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_requests_UserId",
                table: "requests",
                newName: "ix_requests_user_id");

            migrationBuilder.RenameColumn(
                name: "Timeout",
                table: "mocked_errors",
                newName: "timeout");

            migrationBuilder.RenameColumn(
                name: "Method",
                table: "mocked_errors",
                newName: "method");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "mocked_errors",
                newName: "count");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "mocked_errors",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "mocked_errors",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "mocked_errors",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "HttpStatusCode",
                table: "mocked_errors",
                newName: "http_status_code");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "mocked_errors",
                newName: "content_type");

            migrationBuilder.RenameIndex(
                name: "IX_mocked_errors_UserId",
                table: "mocked_errors",
                newName: "ix_mocked_errors_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "game",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "game",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "LaunchName",
                table: "game",
                newName: "launch_name");

            migrationBuilder.RenameColumn(
                name: "GameServerId",
                table: "game",
                newName: "game_server_id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "game",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_LaunchName",
                table: "game",
                newName: "ix_game_launch_name");

            migrationBuilder.RenameIndex(
                name: "IX_game_GameServerId",
                table: "game",
                newName: "ix_game_game_server_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "currencies",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "currencies",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_currencies_Name",
                table: "currencies",
                newName: "ix_currencies_name");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "casinos",
                newName: "provider");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "casinos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SwProviderId",
                table: "casinos",
                newName: "sw_provider_id");

            migrationBuilder.RenameColumn(
                name: "SignatureKey",
                table: "casinos",
                newName: "signature_key");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "casino_games",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "casino_games",
                newName: "game_id");

            migrationBuilder.RenameColumn(
                name: "CasinoId",
                table: "casino_games",
                newName: "casino_id");

            migrationBuilder.RenameIndex(
                name: "IX_casino_games_GameId",
                table: "casino_games",
                newName: "ix_casino_games_game_id");

            migrationBuilder.RenameIndex(
                name: "IX_casino_games_CasinoId",
                table: "casino_games",
                newName: "ix_casino_games_casino_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "casino_currencies",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "casino_currencies",
                newName: "currency_id");

            migrationBuilder.RenameColumn(
                name: "CasinoId",
                table: "casino_currencies",
                newName: "casino_id");

            migrationBuilder.RenameIndex(
                name: "IX_casino_currencies_CurrencyId",
                table: "casino_currencies",
                newName: "ix_casino_currencies_currency_id");

            migrationBuilder.RenameIndex(
                name: "IX_casino_currencies_CasinoId",
                table: "casino_currencies",
                newName: "ix_casino_currencies_casino_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "awards",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                table: "awards",
                newName: "valid_until");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "awards",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_awards_UserId",
                table: "awards",
                newName: "ix_awards_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "award_rounds",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoundId",
                table: "award_rounds",
                newName: "round_id");

            migrationBuilder.RenameColumn(
                name: "AwardId",
                table: "award_rounds",
                newName: "award_id");

            migrationBuilder.RenameIndex(
                name: "IX_award_rounds_RoundId",
                table: "award_rounds",
                newName: "ix_award_rounds_round_id");

            migrationBuilder.RenameIndex(
                name: "IX_award_rounds_AwardId",
                table: "award_rounds",
                newName: "ix_award_rounds_award_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_transactions",
                table: "transactions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_sessions",
                table: "sessions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rounds",
                table: "rounds",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_requests",
                table: "requests",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mocked_errors",
                table: "mocked_errors",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game",
                table: "game",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_currencies",
                table: "currencies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_casinos",
                table: "casinos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_casino_games",
                table: "casino_games",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_casino_currencies",
                table: "casino_currencies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_awards",
                table: "awards",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_award_rounds",
                table: "award_rounds",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_award_rounds_awards_award_id",
                table: "award_rounds",
                column: "award_id",
                principalTable: "awards",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_award_rounds_round_round_id",
                table: "award_rounds",
                column: "round_id",
                principalTable: "rounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_awards_user_user_id",
                table: "awards",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_casino_currencies_casino_casino_id",
                table: "casino_currencies",
                column: "casino_id",
                principalTable: "casinos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_casino_currencies_currency_currency_id",
                table: "casino_currencies",
                column: "currency_id",
                principalTable: "currencies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_casino_games_casino_casino_id",
                table: "casino_games",
                column: "casino_id",
                principalTable: "casinos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_casino_games_game_game_id",
                table: "casino_games",
                column: "game_id",
                principalTable: "game",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mocked_errors_user_user_id",
                table: "mocked_errors",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_requests_user_user_id",
                table: "requests",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rounds_user_user_id",
                table: "rounds",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sessions_user_user_id",
                table: "sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_rounds_round_id",
                table: "transactions",
                column: "round_id",
                principalTable: "rounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_casinos_casino_id",
                table: "users",
                column: "casino_id",
                principalTable: "casinos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_currencies_currency_id",
                table: "users",
                column: "currency_id",
                principalTable: "currencies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_award_rounds_awards_award_id",
                table: "award_rounds");

            migrationBuilder.DropForeignKey(
                name: "fk_award_rounds_round_round_id",
                table: "award_rounds");

            migrationBuilder.DropForeignKey(
                name: "fk_awards_user_user_id",
                table: "awards");

            migrationBuilder.DropForeignKey(
                name: "fk_casino_currencies_casino_casino_id",
                table: "casino_currencies");

            migrationBuilder.DropForeignKey(
                name: "fk_casino_currencies_currency_currency_id",
                table: "casino_currencies");

            migrationBuilder.DropForeignKey(
                name: "fk_casino_games_casino_casino_id",
                table: "casino_games");

            migrationBuilder.DropForeignKey(
                name: "fk_casino_games_game_game_id",
                table: "casino_games");

            migrationBuilder.DropForeignKey(
                name: "fk_mocked_errors_user_user_id",
                table: "mocked_errors");

            migrationBuilder.DropForeignKey(
                name: "fk_requests_user_user_id",
                table: "requests");

            migrationBuilder.DropForeignKey(
                name: "fk_rounds_user_user_id",
                table: "rounds");

            migrationBuilder.DropForeignKey(
                name: "fk_sessions_user_user_id",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_rounds_round_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_users_casinos_casino_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_users_currencies_currency_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_transactions",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_sessions",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rounds",
                table: "rounds");

            migrationBuilder.DropPrimaryKey(
                name: "pk_requests",
                table: "requests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_mocked_errors",
                table: "mocked_errors");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game",
                table: "game");

            migrationBuilder.DropPrimaryKey(
                name: "pk_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "pk_casinos",
                table: "casinos");

            migrationBuilder.DropPrimaryKey(
                name: "pk_casino_games",
                table: "casino_games");

            migrationBuilder.DropPrimaryKey(
                name: "pk_casino_currencies",
                table: "casino_currencies");

            migrationBuilder.DropPrimaryKey(
                name: "pk_awards",
                table: "awards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_award_rounds",
                table: "award_rounds");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "balance",
                table: "users",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "sw_user_id",
                table: "users",
                newName: "SwUserId");

            migrationBuilder.RenameColumn(
                name: "is_disabled",
                table: "users",
                newName: "IsDisabled");

            migrationBuilder.RenameColumn(
                name: "currency_id",
                table: "users",
                newName: "CurrencyId");

            migrationBuilder.RenameColumn(
                name: "casino_id",
                table: "users",
                newName: "CasinoId");

            migrationBuilder.RenameIndex(
                name: "ix_users_user_name",
                table: "users",
                newName: "IX_users_UserName");

            migrationBuilder.RenameIndex(
                name: "ix_users_currency_id",
                table: "users",
                newName: "IX_users_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "ix_users_casino_id",
                table: "users",
                newName: "IX_users_CasinoId");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "transactions",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "internal_id",
                table: "transactions",
                newName: "InternalId");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "transactions",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "ix_transactions_round_id",
                table: "transactions",
                newName: "IX_transactions_RoundId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "sessions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "sessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "expiration_date",
                table: "sessions",
                newName: "ExpirationDate");

            migrationBuilder.RenameIndex(
                name: "ix_sessions_user_id",
                table: "sessions",
                newName: "IX_sessions_UserId");

            migrationBuilder.RenameColumn(
                name: "finished",
                table: "rounds",
                newName: "Finished");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "rounds",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "rounds",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_rounds_user_id",
                table: "rounds",
                newName: "IX_rounds_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "requests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "requests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_requests_user_id",
                table: "requests",
                newName: "IX_requests_UserId");

            migrationBuilder.RenameColumn(
                name: "timeout",
                table: "mocked_errors",
                newName: "Timeout");

            migrationBuilder.RenameColumn(
                name: "method",
                table: "mocked_errors",
                newName: "Method");

            migrationBuilder.RenameColumn(
                name: "count",
                table: "mocked_errors",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "mocked_errors",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "mocked_errors",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "mocked_errors",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "http_status_code",
                table: "mocked_errors",
                newName: "HttpStatusCode");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "mocked_errors",
                newName: "ContentType");

            migrationBuilder.RenameIndex(
                name: "ix_mocked_errors_user_id",
                table: "mocked_errors",
                newName: "IX_mocked_errors_UserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "game",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "game",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "launch_name",
                table: "game",
                newName: "LaunchName");

            migrationBuilder.RenameColumn(
                name: "game_server_id",
                table: "game",
                newName: "GameServerId");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "game",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_game_launch_name",
                table: "game",
                newName: "IX_game_LaunchName");

            migrationBuilder.RenameIndex(
                name: "ix_game_game_server_id",
                table: "game",
                newName: "IX_game_GameServerId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "currencies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "currencies",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "ix_currencies_name",
                table: "currencies",
                newName: "IX_currencies_Name");

            migrationBuilder.RenameColumn(
                name: "provider",
                table: "casinos",
                newName: "Provider");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "casinos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sw_provider_id",
                table: "casinos",
                newName: "SwProviderId");

            migrationBuilder.RenameColumn(
                name: "signature_key",
                table: "casinos",
                newName: "SignatureKey");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "casino_games",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "casino_games",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "casino_id",
                table: "casino_games",
                newName: "CasinoId");

            migrationBuilder.RenameIndex(
                name: "ix_casino_games_game_id",
                table: "casino_games",
                newName: "IX_casino_games_GameId");

            migrationBuilder.RenameIndex(
                name: "ix_casino_games_casino_id",
                table: "casino_games",
                newName: "IX_casino_games_CasinoId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "casino_currencies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "currency_id",
                table: "casino_currencies",
                newName: "CurrencyId");

            migrationBuilder.RenameColumn(
                name: "casino_id",
                table: "casino_currencies",
                newName: "CasinoId");

            migrationBuilder.RenameIndex(
                name: "ix_casino_currencies_currency_id",
                table: "casino_currencies",
                newName: "IX_casino_currencies_CurrencyId");

            migrationBuilder.RenameIndex(
                name: "ix_casino_currencies_casino_id",
                table: "casino_currencies",
                newName: "IX_casino_currencies_CasinoId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "awards",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "valid_until",
                table: "awards",
                newName: "ValidUntil");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "awards",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_awards_user_id",
                table: "awards",
                newName: "IX_awards_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "award_rounds",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "round_id",
                table: "award_rounds",
                newName: "RoundId");

            migrationBuilder.RenameColumn(
                name: "award_id",
                table: "award_rounds",
                newName: "AwardId");

            migrationBuilder.RenameIndex(
                name: "ix_award_rounds_round_id",
                table: "award_rounds",
                newName: "IX_award_rounds_RoundId");

            migrationBuilder.RenameIndex(
                name: "ix_award_rounds_award_id",
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
                name: "PK_requests",
                table: "requests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mocked_errors",
                table: "mocked_errors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game",
                table: "game",
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
                name: "PK_casino_games",
                table: "casino_games",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_casino_currencies",
                table: "casino_currencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_awards",
                table: "awards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_award_rounds",
                table: "award_rounds",
                column: "Id");

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
                name: "FK_casino_games_casinos_CasinoId",
                table: "casino_games",
                column: "CasinoId",
                principalTable: "casinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_casino_games_game_GameId",
                table: "casino_games",
                column: "GameId",
                principalTable: "game",
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
                name: "FK_requests_users_UserId",
                table: "requests",
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
    }
}
