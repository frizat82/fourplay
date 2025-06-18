using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Migrations
{
    /// <inheritdoc />
    public partial class overunder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OverUnder",
                table: "NFLSpreads",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverUnder",
                table: "NFLSpreads");
        }
    }
}
