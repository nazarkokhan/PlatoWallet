using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AwardHasManyRounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_award_rounds_award_id",
                table: "award_rounds");

            migrationBuilder.CreateIndex(
                name: "ix_award_rounds_award_id",
                table: "award_rounds",
                column: "award_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_award_rounds_award_id",
                table: "award_rounds");

            migrationBuilder.CreateIndex(
                name: "ix_award_rounds_award_id",
                table: "award_rounds",
                column: "award_id",
                unique: true);
        }
    }
}
