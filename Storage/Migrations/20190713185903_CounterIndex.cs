using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class CounterIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Counter_HeaterId_ResetDate",
                table: "Counter",
                columns: new[] { "HeaterId", "ResetDate" },
                unique: true,
                filter: "[ResetDate] IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Counter_HeaterId_ResetDate",
                table: "Counter");
        }
    }
}
