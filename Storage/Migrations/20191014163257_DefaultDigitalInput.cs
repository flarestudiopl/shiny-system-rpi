using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class DefaultDigitalInput : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DigitalInput",
                columns: new[] { "DigitalInputId", "BuildingId", "DeviceId", "Function", "InputName", "Inverted", "ProtocolName" },
                values: new object[] { -1, -1, 0, 0, "AC OK", true, "Shiny HW 1.1" });

            migrationBuilder.InsertData(
                table: "DigitalInput",
                columns: new[] { "DigitalInputId", "BuildingId", "DeviceId", "Function", "InputName", "Inverted", "ProtocolName" },
                values: new object[] { -2, -1, 0, 1, "Low bat", false, "Shiny HW 1.1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DigitalInput",
                keyColumn: "DigitalInputId",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "DigitalInput",
                keyColumn: "DigitalInputId",
                keyValue: -1);
        }
    }
}
