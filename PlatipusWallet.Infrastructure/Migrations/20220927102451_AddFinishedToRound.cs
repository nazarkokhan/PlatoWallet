using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatipusWallet.Infrastructure.Migrations
{
    public partial class AddFinishedToRound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Finished",
                table: "Rounds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Finished",
                table: "Rounds");
        }
    }
}
