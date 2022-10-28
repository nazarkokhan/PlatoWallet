using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class AddCasinoProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "Casinos",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Casinos");
        }
    }
}
