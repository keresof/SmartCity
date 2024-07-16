using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeLocationAndCoordinatesSeparate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Location",
                table: "Reports",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal[]>(
                name: "Coordinates",
                table: "Reports",
                type: "numeric[]",
                nullable: false,
                defaultValue: new decimal[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Reports");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Reports",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }
    }
}
