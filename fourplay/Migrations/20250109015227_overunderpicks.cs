using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fourplay.Migrations
{
    /// <inheritdoc />
    public partial class overunderpicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JuiceConference",
                table: "LeagueJuiceMapping",
                type: "integer",
                nullable: false,
                defaultValue: 6);

            migrationBuilder.AddColumn<int>(
                name: "JuiceDivisonal",
                table: "LeagueJuiceMapping",
                type: "integer",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.CreateTable(
                name: "NFLPostSeasonPicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeagueId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    HomeTeam = table.Column<string>(type: "text", nullable: false),
                    AwayTeam = table.Column<string>(type: "text", nullable: false),
                    Pick = table.Column<int>(type: "integer", nullable: false),
                    NFLWeek = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFLPostSeasonPicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NFLPostSeasonPicks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NFLPostSeasonPicks_LeagueInfo_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "LeagueInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NFLPostSeasonPicks_LeagueId",
                table: "NFLPostSeasonPicks",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_NFLPostSeasonPicks_UserId_LeagueId_NFLWeek_Season_HomeTeam_~",
                table: "NFLPostSeasonPicks",
                columns: new[] { "UserId", "LeagueId", "NFLWeek", "Season", "HomeTeam", "AwayTeam" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NFLPostSeasonPicks");

            migrationBuilder.DropColumn(
                name: "JuiceConference",
                table: "LeagueJuiceMapping");

            migrationBuilder.DropColumn(
                name: "JuiceDivisonal",
                table: "LeagueJuiceMapping");
        }
    }
}
