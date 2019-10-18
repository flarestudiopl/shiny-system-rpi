using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class SpeedUpLoops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Building",
                keyColumn: "BuildingId",
                keyValue: -1,
                column: "ControlLoopIntervalSecondsMilliseconds",
                value: 3000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Building",
                keyColumn: "BuildingId",
                keyValue: -1,
                column: "ControlLoopIntervalSecondsMilliseconds",
                value: 5000);
        }
    }
}
