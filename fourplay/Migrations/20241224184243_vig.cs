using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fourplay.Migrations {
    /// <inheritdoc />
    public partial class vig : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<int>(
                name: "WeeklyCost",
                table: "LeagueJuiceMapping",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            // Clear the LeagueUsers table
            migrationBuilder.DropTable(
                           name: "LeagueUsers");
            migrationBuilder.CreateTable(
                           name: "LeagueUsers",
                           columns: table => new {
                               Id = table.Column<int>(type: "integer", nullable: false)
                                   .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                               GoogleEmail = table.Column<string>(type: "text", nullable: false)
                           },
                           constraints: table => {
                               table.PrimaryKey("PK_LeagueUsers", x => x.Id);
                           });
            migrationBuilder.CreateIndex(
                name: "IX_LeagueUsers_GoogleEmail",
                table: "LeagueUsers",
                column: "GoogleEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_LeagueUsers_GoogleEmail",
                table: "LeagueUsers");

            migrationBuilder.DropColumn(
                name: "WeeklyCost",
                table: "LeagueJuiceMapping");
        }
    }
}
