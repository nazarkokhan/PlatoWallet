using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    public partial class NumbersScaleConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Users",
                type: "numeric(38,2)",
                precision: 38,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "numeric(38,2)",
                precision: 38,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Users",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,2)",
                oldPrecision: 38,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,2)",
                oldPrecision: 38,
                oldScale: 2);
        }
    }
}
