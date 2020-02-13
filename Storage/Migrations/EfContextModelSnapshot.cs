﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Storage.StorageDatabase;

namespace Storage.Migrations
{
    [DbContext(typeof(EfContext))]
    partial class EfContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("Domain.AuditLog", b =>
                {
                    b.Property<int>("AuditLogId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EventTs");

                    b.Property<string>("KeyValues");

                    b.Property<string>("NewValues");

                    b.Property<string>("OldValues");

                    b.Property<string>("TableName");

                    b.HasKey("AuditLogId");

                    b.ToTable("AuditLog");
                });

            modelBuilder.Entity("Domain.Building", b =>
                {
                    b.Property<int>("BuildingId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ControlLoopIntervalSecondsMilliseconds");

                    b.Property<bool>("IsDefault");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("BuildingId");

                    b.ToTable("Building");

                    b.HasData(
                        new
                        {
                            BuildingId = -1,
                            ControlLoopIntervalSecondsMilliseconds = 2500,
                            IsDefault = true,
                            Name = "Budynek testowy"
                        });
                });

            modelBuilder.Entity("Domain.Counter", b =>
                {
                    b.Property<int>("CounterId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CountedSeconds");

                    b.Property<int>("HeaterId");

                    b.Property<DateTime?>("ResetDate");

                    b.Property<int?>("ResettedByUserId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("CounterId");

                    b.HasIndex("ResettedByUserId");

                    b.HasIndex("HeaterId", "ResetDate")
                        .IsUnique()
                        .HasFilter("[ResetDate] IS NULL");

                    b.ToTable("Counter");
                });

            modelBuilder.Entity("Domain.DigitalInput", b =>
                {
                    b.Property<int>("DigitalInputId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuildingId");

                    b.Property<int>("DeviceId");

                    b.Property<int>("Function");

                    b.Property<string>("InputName");

                    b.Property<bool>("Inverted");

                    b.Property<string>("ProtocolName");

                    b.HasKey("DigitalInputId");

                    b.HasIndex("BuildingId");

                    b.ToTable("DigitalInput");

                    b.HasData(
                        new
                        {
                            DigitalInputId = -1,
                            BuildingId = -1,
                            DeviceId = 0,
                            Function = 0,
                            InputName = "AC OK",
                            Inverted = true,
                            ProtocolName = "Shiny HW 1.1"
                        },
                        new
                        {
                            DigitalInputId = -2,
                            BuildingId = -1,
                            DeviceId = 0,
                            Function = 1,
                            InputName = "Low bat",
                            Inverted = false,
                            ProtocolName = "Shiny HW 1.1"
                        });
                });

            modelBuilder.Entity("Domain.DigitalOutput", b =>
                {
                    b.Property<int>("DigitalOutputId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OutputDescriptor");

                    b.Property<string>("ProtocolName");

                    b.HasKey("DigitalOutputId");

                    b.ToTable("DigitalOutput");
                });

            modelBuilder.Entity("Domain.Heater", b =>
                {
                    b.Property<int>("HeaterId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuildingId");

                    b.Property<int>("DigitalOutputId");

                    b.Property<int>("MinimumStateChangeIntervalSeconds");

                    b.Property<string>("Name");

                    b.Property<int?>("PowerZoneId");

                    b.Property<decimal>("UsagePerHour");

                    b.Property<byte>("UsageUnit");

                    b.Property<int?>("ZoneId");

                    b.HasKey("HeaterId");

                    b.HasIndex("BuildingId");

                    b.HasIndex("DigitalOutputId")
                        .IsUnique();

                    b.HasIndex("PowerZoneId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Heater");
                });

            modelBuilder.Entity("Domain.PowerZone", b =>
                {
                    b.Property<int>("PowerZoneId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuildingId");

                    b.Property<decimal>("MaxUsage");

                    b.Property<string>("Name");

                    b.Property<int>("RoundRobinIntervalMinutes");

                    b.Property<int>("SwitchDelayBetweenOutputsSeconds")
                        .HasDefaultValue(2);

                    b.Property<byte>("UsageUnit");

                    b.HasKey("PowerZoneId");

                    b.HasIndex("BuildingId");

                    b.ToTable("PowerZone");
                });

            modelBuilder.Entity("Domain.ScheduleItem", b =>
                {
                    b.Property<int>("ScheduleItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("BeginTime");

                    b.Property<string>("DaysOfWeek")
                        .IsRequired();

                    b.Property<TimeSpan>("EndTime");

                    b.Property<float?>("SetPoint");

                    b.Property<int>("ZoneId");

                    b.HasKey("ScheduleItemId");

                    b.HasIndex("ZoneId");

                    b.ToTable("ScheduleItem");
                });

            modelBuilder.Entity("Domain.TemperatureControlledZone", b =>
                {
                    b.Property<int>("TemperatureControlledZoneId")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("HighSetPoint");

                    b.Property<float>("Hysteresis");

                    b.Property<float>("LowSetPoint");

                    b.Property<float>("ScheduleDefaultSetPoint");

                    b.Property<int>("TemperatureSensorId");

                    b.HasKey("TemperatureControlledZoneId");

                    b.HasIndex("TemperatureSensorId");

                    b.ToTable("TemperatureControlledZone");
                });

            modelBuilder.Entity("Domain.TemperatureSensor", b =>
                {
                    b.Property<int>("TemperatureSensorId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuildingId");

                    b.Property<string>("InputDescriptor")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ProtocolName")
                        .IsRequired();

                    b.HasKey("TemperatureSensorId");

                    b.HasIndex("BuildingId");

                    b.ToTable("TemperatureSensor");
                });

            modelBuilder.Entity("Domain.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int?>("DisabledByUserId");

                    b.Property<DateTime?>("DisabledDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsBrowseable");

                    b.Property<DateTime?>("LastLogonDate");

                    b.Property<string>("LastSeenIpAddress");

                    b.Property<string>("Login")
                        .IsRequired();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("QuickLoginPinHash");

                    b.HasKey("UserId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("DisabledByUserId");

                    b.HasIndex("Login", "IsActive")
                        .IsUnique()
                        .HasFilter("[IsActive] = 1");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            UserId = -1,
                            CreatedDate = new DateTime(2019, 7, 10, 18, 4, 28, 876, DateTimeKind.Utc).AddTicks(8468),
                            IsActive = true,
                            IsBrowseable = true,
                            Login = "user",
                            PasswordHash = "3da810408ed48e255a05f80798db255a4ae32b205e895f08ffe0833338d03d71"
                        });
                });

            modelBuilder.Entity("Domain.UserPermission", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("Permission");

                    b.HasKey("UserId", "Permission");

                    b.ToTable("UserPermission");

                    b.HasData(
                        new
                        {
                            UserId = -1,
                            Permission = 100
                        },
                        new
                        {
                            UserId = -1,
                            Permission = 101
                        },
                        new
                        {
                            UserId = -1,
                            Permission = 200
                        },
                        new
                        {
                            UserId = -1,
                            Permission = 300
                        },
                        new
                        {
                            UserId = -1,
                            Permission = 400
                        },
                        new
                        {
                            UserId = -1,
                            Permission = 500
                        });
                });

            modelBuilder.Entity("Domain.Zone", b =>
                {
                    b.Property<int>("ZoneId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BuildingId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("TemperatureControlledZoneId");

                    b.HasKey("ZoneId");

                    b.HasIndex("BuildingId");

                    b.HasIndex("TemperatureControlledZoneId")
                        .IsUnique();

                    b.ToTable("Zone");
                });

            modelBuilder.Entity("Domain.Counter", b =>
                {
                    b.HasOne("Domain.User", "ResettedBy")
                        .WithMany()
                        .HasForeignKey("ResettedByUserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Domain.DigitalInput", b =>
                {
                    b.HasOne("Domain.Building")
                        .WithMany("DigitalInputs")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Heater", b =>
                {
                    b.HasOne("Domain.Building")
                        .WithMany("Heaters")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.DigitalOutput", "DigitalOutput")
                        .WithOne()
                        .HasForeignKey("Domain.Heater", "DigitalOutputId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.PowerZone", "PowerZone")
                        .WithMany("Heaters")
                        .HasForeignKey("PowerZoneId");

                    b.HasOne("Domain.Zone", "Zone")
                        .WithMany("Heaters")
                        .HasForeignKey("ZoneId");
                });

            modelBuilder.Entity("Domain.PowerZone", b =>
                {
                    b.HasOne("Domain.Building")
                        .WithMany("PowerZones")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.ScheduleItem", b =>
                {
                    b.HasOne("Domain.Zone")
                        .WithMany("Schedule")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.TemperatureControlledZone", b =>
                {
                    b.HasOne("Domain.TemperatureSensor", "TemperatureSensor")
                        .WithMany("TemperatureControlledZones")
                        .HasForeignKey("TemperatureSensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.TemperatureSensor", b =>
                {
                    b.HasOne("Domain.Building")
                        .WithMany("TemperatureSensors")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.User", b =>
                {
                    b.HasOne("Domain.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.User", "DisabledBy")
                        .WithMany()
                        .HasForeignKey("DisabledByUserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Domain.UserPermission", b =>
                {
                    b.HasOne("Domain.User", "User")
                        .WithMany("UserPermissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.Zone", b =>
                {
                    b.HasOne("Domain.Building")
                        .WithMany("Zones")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Domain.TemperatureControlledZone", "TemperatureControlledZone")
                        .WithOne()
                        .HasForeignKey("Domain.Zone", "TemperatureControlledZoneId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
