using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class PowerOptionsPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into UserPermission (UserId, Permission) select UserId, 401 from User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete UserPermission where Permission = 401");
        }
    }
}
