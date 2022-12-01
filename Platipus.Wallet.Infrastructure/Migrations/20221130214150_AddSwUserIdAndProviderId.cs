using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class AddSwUserIdAndProviderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SwUserId",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SwProviderId",
                table: "casinos",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SwUserId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "SwProviderId",
                table: "casinos");
        }
    }
}
