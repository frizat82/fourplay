using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Data.Migrations
{
    /// <inheritdoc />
    public partial class userkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NFLPicks_UserId_NFLWeek",
                table: "NFLPicks");

            migrationBuilder.CreateIndex(
                name: "IX_NFLPicks_UserId_LeagueId_NFLWeek_Season_Team",
                table: "NFLPicks",
                columns: new[] { "UserId", "LeagueId", "NFLWeek", "Season", "Team" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NFLPicks_UserId_LeagueId_NFLWeek_Season_Team",
                table: "NFLPicks");

            migrationBuilder.CreateIndex(
                name: "IX_NFLPicks_UserId_NFLWeek",
                table: "NFLPicks",
                columns: new[] { "UserId", "NFLWeek" },
                unique: true);
        }
    }
}
