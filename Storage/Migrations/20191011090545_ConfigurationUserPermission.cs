using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class ConfigurationUserPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserPermission",
                columns: new[] { "UserId", "Permission" },
                values: new object[] { -1, 500 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserPermission",
                keyColumns: new[] { "UserId", "Permission" },
                keyValues: new object[] { -1, 500 });
        }
    }
}
