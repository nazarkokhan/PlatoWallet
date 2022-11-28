using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class AddTransactionInternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalId",
                table: "transactions",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"

UPDATE transactions
SET ""InternalId"" = gen_random_uuid()

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalId",
                table: "transactions");
        }
    }
}
