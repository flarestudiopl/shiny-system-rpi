using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class ZoneNameDashboardEn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameDashboardEn",
                table: "Zone",
                nullable: true);

            migrationBuilder.Sql("update Zone set NameDashboardEn = Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameDashboardEn",
                table: "Zone");
        }
    }
}
