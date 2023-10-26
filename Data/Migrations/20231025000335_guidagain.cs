using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourplay.Data.Migrations
{
    /// <inheritdoc />
    public partial class guidagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLSpreads",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("209013f2-dc78-41e4-938b-4615b71d388f"),
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLScores",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("c38d476e-42a0-482e-b895-04be96029a4b"),
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLPicks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("a114a9e7-da03-40f3-8687-2bf0b59b80af"),
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLSpreads",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("209013f2-dc78-41e4-938b-4615b71d388f"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLScores",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("c38d476e-42a0-482e-b895-04be96029a4b"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "NFLPicks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("a114a9e7-da03-40f3-8687-2bf0b59b80af"));
        }
    }
}
