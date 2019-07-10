using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class UserAndCounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    LastSeenIpAddress = table.Column<string>(nullable: true),
                    LastLogonDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsBrowseable = table.Column<bool>(nullable: false),
                    CreatedByUserId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DisabledByUserId = table.Column<int>(nullable: true),
                    DisabledDate = table.Column<DateTime>(nullable: true),
                    QuickLoginPinHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_User_DisabledByUserId",
                        column: x => x.DisabledByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Counter",
                columns: table => new
                {
                    CounterId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeaterId = table.Column<int>(nullable: false),
                    CountedSeconds = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    ResetDate = table.Column<DateTime>(nullable: true),
                    ResettedByUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counter", x => x.CounterId);
                    table.ForeignKey(
                        name: "FK_Counter_User_ResettedByUserId",
                        column: x => x.ResettedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedByUserId", "CreatedDate", "DisabledByUserId", "DisabledDate", "IsActive", "IsBrowseable", "LastLogonDate", "LastSeenIpAddress", "Login", "PasswordHash", "QuickLoginPinHash" },
                values: new object[] { -1, null, new DateTime(2019, 7, 10, 18, 4, 28, 876, DateTimeKind.Utc).AddTicks(8468), null, null, true, true, null, null, "user", "3da810408ed48e255a05f80798db255a4ae32b205e895f08ffe0833338d03d71", null });

            migrationBuilder.CreateIndex(
                name: "IX_Counter_ResettedByUserId",
                table: "Counter",
                column: "ResettedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedByUserId",
                table: "User",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DisabledByUserId",
                table: "User",
                column: "DisabledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Login_IsActive",
                table: "User",
                columns: new[] { "Login", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Counter");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
