using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class UserPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPermission",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Permission = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermission", x => new { x.UserId, x.Permission });
                    table.ForeignKey(
                        name: "FK_UserPermission_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserPermission",
                columns: new[] { "UserId", "Permission" },
                values: new object[] { -1, 100 });

            migrationBuilder.InsertData(
                table: "UserPermission",
                columns: new[] { "UserId", "Permission" },
                values: new object[] { -1, 101 });

            migrationBuilder.InsertData(
                table: "UserPermission",
                columns: new[] { "UserId", "Permission" },
                values: new object[] { -1, 200 });

            migrationBuilder.InsertData(
                table: "UserPermission",
                columns: new[] { "UserId", "Permission" },
                values: new object[] { -1, 300 });

            migrationBuilder.InsertData(
                table: "UserPermission",
                columns: new[] { "UserId", "Permission" },
                values: new object[] { -1, 400 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermission");
        }
    }
}
