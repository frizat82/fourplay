using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Data.Migrations {
    /// <inheritdoc />
    public partial class ints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLSpreads",
                table: "NFLSpreads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLScores",
                table: "NFLScores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLPicks",
                table: "NFLPicks");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NFLSpreads",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("209013f2-dc78-41e4-938b-4615b71d388f"))
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NFLScores",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("c38d476e-42a0-482e-b895-04be96029a4b"))
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NFLPicks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("a114a9e7-da03-40f3-8687-2bf0b59b80af"))
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "NFLPicks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLSpreads",
                table: "NFLSpreads",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLScores",
                table: "NFLScores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLPicks",
                table: "NFLPicks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LeagueInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueName = table.Column<string>(type: "TEXT", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Juice = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DateUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NFLSpreads_Season_NFLWeek_HomeTeam",
                table: "NFLSpreads",
                columns: new[] { "Season", "NFLWeek", "HomeTeam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NFLScores_Season_NFLWeek_HomeTeam",
                table: "NFLScores",
                columns: new[] { "Season", "NFLWeek", "HomeTeam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NFLPicks_LeagueId",
                table: "NFLPicks",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_NFLPicks_UserId_NFLWeek",
                table: "NFLPicks",
                columns: new[] { "UserId", "NFLWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueInfo_LeagueName_Season",
                table: "LeagueInfo",
                columns: new[] { "LeagueName", "Season" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NFLPicks_LeagueInfo_LeagueId",
                table: "NFLPicks",
                column: "LeagueId",
                principalTable: "LeagueInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NFLPicks_LeagueInfo_LeagueId",
                table: "NFLPicks");

            migrationBuilder.DropTable(
                name: "LeagueInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLSpreads",
                table: "NFLSpreads");

            migrationBuilder.DropIndex(
                name: "IX_NFLSpreads_Season_NFLWeek_HomeTeam",
                table: "NFLSpreads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLScores",
                table: "NFLScores");

            migrationBuilder.DropIndex(
                name: "IX_NFLScores_Season_NFLWeek_HomeTeam",
                table: "NFLScores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NFLPicks",
                table: "NFLPicks");

            migrationBuilder.DropIndex(
                name: "IX_NFLPicks_LeagueId",
                table: "NFLPicks");

            migrationBuilder.DropIndex(
                name: "IX_NFLPicks_UserId_NFLWeek",
                table: "NFLPicks");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "NFLPicks");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLSpreads",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("209013f2-dc78-41e4-938b-4615b71d388f"),
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLScores",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("c38d476e-42a0-482e-b895-04be96029a4b"),
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLPicks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("a114a9e7-da03-40f3-8687-2bf0b59b80af"),
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLSpreads",
                table: "NFLSpreads",
                columns: new[] { "NFLWeek", "HomeTeam" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLScores",
                table: "NFLScores",
                columns: new[] { "Season", "NFLWeek", "HomeTeam" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NFLPicks",
                table: "NFLPicks",
                columns: new[] { "UserId", "NFLWeek" });
        }
    }
}
