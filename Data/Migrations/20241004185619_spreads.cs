using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Data.Migrations
{
    /// <inheritdoc />
    public partial class spreads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FourPlayAwaySpread",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "FourPlayHomeSpread",
                table: "NFLSpreads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FourPlayAwaySpread",
                table: "NFLSpreads",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FourPlayHomeSpread",
                table: "NFLSpreads",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
