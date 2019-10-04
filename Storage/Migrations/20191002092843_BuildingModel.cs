using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Storage.Migrations
{
    public partial class BuildingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Building",
                columns: table => new
                {
                    BuildingId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    ControlLoopIntervalSecondsMilliseconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.BuildingId);
                });

            migrationBuilder.CreateTable(
                name: "DigitalOutput",
                columns: table => new
                {
                    DigitalOutputId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProtocolName = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    OutputChannel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalOutput", x => x.DigitalOutputId);
                });

            migrationBuilder.CreateTable(
                name: "DigitalInput",
                columns: table => new
                {
                    DigitalInputId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildingId = table.Column<int>(nullable: false),
                    ProtocolName = table.Column<string>(nullable: true),
                    DeviceId = table.Column<int>(nullable: false),
                    InputName = table.Column<string>(nullable: true),
                    Function = table.Column<int>(nullable: false),
                    Inverted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalInput", x => x.DigitalInputId);
                    table.ForeignKey(
                        name: "FK_DigitalInput_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerZone",
                columns: table => new
                {
                    PowerZoneId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildingId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MaxUsage = table.Column<decimal>(nullable: false),
                    UsageUnit = table.Column<byte>(nullable: false),
                    RoundRobinIntervalMinutes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerZone", x => x.PowerZoneId);
                    table.ForeignKey(
                        name: "FK_PowerZone_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureSensor",
                columns: table => new
                {
                    TemperatureSensorId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildingId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DeviceId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureSensor", x => x.TemperatureSensorId);
                    table.ForeignKey(
                        name: "FK_TemperatureSensor_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemperatureControlledZone",
                columns: table => new
                {
                    TemperatureControlledZoneId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LowSetPoint = table.Column<float>(nullable: false),
                    HighSetPoint = table.Column<float>(nullable: false),
                    ScheduleDefaultSetPoint = table.Column<float>(nullable: false),
                    Hysteresis = table.Column<float>(nullable: false),
                    TemperatureSensorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureControlledZone", x => x.TemperatureControlledZoneId);
                    table.ForeignKey(
                        name: "FK_TemperatureControlledZone_TemperatureSensor_TemperatureSensorId",
                        column: x => x.TemperatureSensorId,
                        principalTable: "TemperatureSensor",
                        principalColumn: "TemperatureSensorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                columns: table => new
                {
                    ZoneId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildingId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TemperatureControlledZoneId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.ZoneId);
                    table.ForeignKey(
                        name: "FK_Zone_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zone_TemperatureControlledZone_TemperatureControlledZoneId",
                        column: x => x.TemperatureControlledZoneId,
                        principalTable: "TemperatureControlledZone",
                        principalColumn: "TemperatureControlledZoneId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Heater",
                columns: table => new
                {
                    HeaterId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuildingId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DigitalOutputId = table.Column<int>(nullable: false),
                    UsageUnit = table.Column<byte>(nullable: false),
                    UsagePerHour = table.Column<decimal>(nullable: false),
                    MinimumStateChangeIntervalSeconds = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: true),
                    PowerZoneId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heater", x => x.HeaterId);
                    table.ForeignKey(
                        name: "FK_Heater_Building_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "BuildingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heater_DigitalOutput_DigitalOutputId",
                        column: x => x.DigitalOutputId,
                        principalTable: "DigitalOutput",
                        principalColumn: "DigitalOutputId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heater_PowerZone_PowerZoneId",
                        column: x => x.PowerZoneId,
                        principalTable: "PowerZone",
                        principalColumn: "PowerZoneId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Heater_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleItem",
                columns: table => new
                {
                    ScheduleItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ZoneId = table.Column<int>(nullable: false),
                    DaysOfWeek = table.Column<string>(nullable: false),
                    BeginTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false),
                    SetPoint = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleItem", x => x.ScheduleItemId);
                    table.ForeignKey(
                        name: "FK_ScheduleItem_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Building",
                columns: new[] { "BuildingId", "ControlLoopIntervalSecondsMilliseconds", "IsDefault", "Name" },
                values: new object[] { -1, 5000, true, "Budynek testowy" });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalInput_BuildingId",
                table: "DigitalInput",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Heater_BuildingId",
                table: "Heater",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Heater_DigitalOutputId",
                table: "Heater",
                column: "DigitalOutputId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Heater_PowerZoneId",
                table: "Heater",
                column: "PowerZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Heater_ZoneId",
                table: "Heater",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerZone_BuildingId",
                table: "PowerZone",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleItem_ZoneId",
                table: "ScheduleItem",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureControlledZone_TemperatureSensorId",
                table: "TemperatureControlledZone",
                column: "TemperatureSensorId");

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureSensor_BuildingId",
                table: "TemperatureSensor",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_BuildingId",
                table: "Zone",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_TemperatureControlledZoneId",
                table: "Zone",
                column: "TemperatureControlledZoneId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DigitalInput");

            migrationBuilder.DropTable(
                name: "Heater");

            migrationBuilder.DropTable(
                name: "ScheduleItem");

            migrationBuilder.DropTable(
                name: "DigitalOutput");

            migrationBuilder.DropTable(
                name: "PowerZone");

            migrationBuilder.DropTable(
                name: "Zone");

            migrationBuilder.DropTable(
                name: "TemperatureControlledZone");

            migrationBuilder.DropTable(
                name: "TemperatureSensor");

            migrationBuilder.DropTable(
                name: "Building");
        }
    }
}
