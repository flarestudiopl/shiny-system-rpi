using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class ZoneSwitchDelay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SwitchDelayBetweenOutputsSeconds",
                table: "Zone",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.UpdateData(
                table: "Building",
                keyColumn: "BuildingId",
                keyValue: -1,
                column: "ControlLoopIntervalSecondsMilliseconds",
                value: 2500);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SwitchDelayBetweenOutputsSeconds",
                table: "Zone");

            migrationBuilder.UpdateData(
                table: "Building",
                keyColumn: "BuildingId",
                keyValue: -1,
                column: "ControlLoopIntervalSecondsMilliseconds",
                value: 3000);
        }
    }
}
