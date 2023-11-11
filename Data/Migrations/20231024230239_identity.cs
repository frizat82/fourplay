using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Data.Migrations {
    /// <inheritdoc />
    public partial class identity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "NFLSpreads",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "NFLScores",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "NFLPicks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "NFLSpreads");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "NFLScores");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "NFLPicks");
        }
    }
}
