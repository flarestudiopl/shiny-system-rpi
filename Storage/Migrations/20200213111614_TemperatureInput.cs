using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class TemperatureInput : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "TemperatureSensor",
                newName: "ProtocolName");

            migrationBuilder.AddColumn<string>(
                name: "InputDescriptor",
                table: "TemperatureSensor",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE TemperatureSensor SET InputDescriptor = '{ \"DeviceId\": \"' || ProtocolName || '\" }'");
            migrationBuilder.Sql("UPDATE TemperatureSensor SET ProtocolName = 'DS1820'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputDescriptor",
                table: "TemperatureSensor");

            migrationBuilder.RenameColumn(
                name: "ProtocolName",
                table: "TemperatureSensor",
                newName: "DeviceId");
        }
    }
}
