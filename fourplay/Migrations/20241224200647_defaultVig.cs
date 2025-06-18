using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Migrations
{
    /// <inheritdoc />
    public partial class defaultVig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WeeklyCost",
                table: "LeagueJuiceMapping",
                type: "integer",
                nullable: false,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WeeklyCost",
                table: "LeagueJuiceMapping",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 5);
        }
    }
}
