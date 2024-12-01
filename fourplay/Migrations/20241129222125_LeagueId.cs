using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Migrations
{
    /// <inheritdoc />
    public partial class LeagueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeagueInfo_LeagueName_Season",
                table: "LeagueInfo");

            migrationBuilder.DropColumn(
                name: "Juice",
                table: "LeagueInfo");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "LeagueInfo");

            migrationBuilder.CreateTable(
                name: "LeagueJuiceMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Juice = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueJuiceMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeagueJuiceMapping_LeagueInfo_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "LeagueInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueInfo_LeagueName",
                table: "LeagueInfo",
                column: "LeagueName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueJuiceMapping_LeagueId_Season",
                table: "LeagueJuiceMapping",
                columns: new[] { "LeagueId", "Season" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeagueJuiceMapping");

            migrationBuilder.DropIndex(
                name: "IX_LeagueInfo_LeagueName",
                table: "LeagueInfo");

            migrationBuilder.AddColumn<int>(
                name: "Juice",
                table: "LeagueInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "LeagueInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueInfo_LeagueName_Season",
                table: "LeagueInfo",
                columns: new[] { "LeagueName", "Season" },
                unique: true);
        }
    }
}
