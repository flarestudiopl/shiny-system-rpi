using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class OutputDescriptor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OutputDescriptor",
                table: "DigitalOutput",
                nullable: true);

            migrationBuilder.Sql("UPDATE DigitalOutput SET OutputDescriptor = '{ \"DeviceId\": ' || DeviceId || ', \"OutputName\": \"' || OutputChannel || '\" }'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputDescriptor",
                table: "DigitalOutput");
        }
    }
}
