using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Migrations
{
    /// <inheritdoc />
    public partial class vig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeeklyCost",
                table: "LeagueJuiceMapping",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUsers_GoogleEmail",
                table: "LeagueUsers",
                column: "GoogleEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeagueUsers_GoogleEmail",
                table: "LeagueUsers");

            migrationBuilder.DropColumn(
                name: "WeeklyCost",
                table: "LeagueJuiceMapping");
        }
    }
}
