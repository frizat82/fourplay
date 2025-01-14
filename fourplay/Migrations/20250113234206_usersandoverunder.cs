using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Migrations
{
    /// <inheritdoc />
    public partial class usersandoverunder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NFLPostSeasonPicks_UserId_LeagueId_NFLWeek_Season_HomeTeam_~",
                table: "NFLPostSeasonPicks");

            migrationBuilder.DropColumn(
                name: "AwayTeam",
                table: "NFLPostSeasonPicks");

            migrationBuilder.RenameColumn(
                name: "HomeTeam",
                table: "NFLPostSeasonPicks",
                newName: "Team");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NickName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NFLPostSeasonPicks_UserId_LeagueId_NFLWeek_Season_Team_Pick",
                table: "NFLPostSeasonPicks",
                columns: new[] { "UserId", "LeagueId", "NFLWeek", "Season", "Team", "Pick" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NFLPostSeasonPicks_UserId_LeagueId_NFLWeek_Season_Team_Pick",
                table: "NFLPostSeasonPicks");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NickName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Team",
                table: "NFLPostSeasonPicks",
                newName: "HomeTeam");

            migrationBuilder.AddColumn<string>(
                name: "AwayTeam",
                table: "NFLPostSeasonPicks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NFLPostSeasonPicks_UserId_LeagueId_NFLWeek_Season_HomeTeam_~",
                table: "NFLPostSeasonPicks",
                columns: new[] { "UserId", "LeagueId", "NFLWeek", "Season", "HomeTeam", "AwayTeam" },
                unique: true);
        }
    }
}
