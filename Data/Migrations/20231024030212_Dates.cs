using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Data.Migrations {
    /// <inheritdoc />
    public partial class Dates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Spread",
                table: "NFLSpreads",
                newName: "GameTime");

            migrationBuilder.AddColumn<double>(
                name: "AwayTeamSpread",
                table: "NFLSpreads",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

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

            migrationBuilder.AddColumn<double>(
                name: "HomeTeamSpread",
                table: "NFLSpreads",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "NFLSpreads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "NFLPicks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NFLScores",
                columns: table => new
                {
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    NFLWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeam = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeam = table.Column<string>(type: "TEXT", nullable: false),
                    HomeTeamScore = table.Column<double>(type: "REAL", nullable: false),
                    AwayTeamScore = table.Column<double>(type: "REAL", nullable: false),
                    GameTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFLScores", x => new { x.Season, x.NFLWeek, x.HomeTeam });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NFLScores");

            migrationBuilder.DropColumn(
                name: "AwayTeamSpread",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "FourPlayAwaySpread",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "FourPlayHomeSpread",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "HomeTeamSpread",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "NFLPicks");

            migrationBuilder.RenameColumn(
                name: "GameTime",
                table: "NFLSpreads",
                newName: "Spread");
        }
    }
}
