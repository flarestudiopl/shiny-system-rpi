using Commons.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Storage.StorageDatabase
{
    public class EfContext : DbContext
    {
        private readonly string _connectionString;
        private readonly bool _persistOnSave;

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<DigitalInput> DigitalInputs { get; set; }
        public DbSet<DigitalOutput> DigitalOutputs { get; set; }
        public DbSet<Heater> Heaters { get; set; }
        public DbSet<PowerZone> PowerZones { get; set; }
        public DbSet<TemperatureSensor> TemperatureSensors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public EfContext()
        {
            _connectionString = "Filename=test.db";
        }

        public EfContext(string connectionString, bool persistOnSave)
        {
            _connectionString = connectionString;
            _persistOnSave = persistOnSave;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        public override int SaveChanges()
        {
            if (!_persistOnSave)
            {
                return 0;
            }

            var result = base.SaveChanges(false);

            var auditLogs = GetAuditLogs().ToList();
            ChangeTracker.AcceptAllChanges();

            AuditLogs.AddRange(auditLogs);
            base.SaveChanges();

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Building>(x =>
            {
                x.ToTable(nameof(Building));

                x.Property(b => b.Name).IsRequired();
                x.Property(b => b.IsDefault).IsRequired();
                x.Property(b => b.ControlLoopIntervalSecondsMilliseconds).IsRequired();

                x.HasData(new Building
                {
                    BuildingId = -1,
                    ControlLoopIntervalSecondsMilliseconds = 2500,
                    IsDefault = true,
                    Name = "Budynek testowy"
                });
            });

            modelBuilder.Entity<Counter>(x =>
            {
                x.ToTable(nameof(Counter));

                x.HasOne(c => c.ResettedBy).WithMany().HasForeignKey(c => c.ResettedByUserId).OnDelete(DeleteBehavior.Restrict);

                x.HasIndex(c => new { c.HeaterId, c.ResetDate }).HasFilter($"[{nameof(Counter.ResetDate)}] IS NULL").IsUnique();
            });

            modelBuilder.Entity<DigitalInput>(x =>
            {
                x.ToTable(nameof(DigitalInput));

                x.HasOne<Building>().WithMany(b => b.DigitalInputs).HasForeignKey(di => di.BuildingId);

                x.HasData(new DigitalInput { DigitalInputId = -1, BuildingId = -1, ProtocolName = ProtocolNames.ShinyBoard, DeviceId = 0, InputName = "AC OK", Function = DigitalInputFunction.BatteryMode, Inverted = true },
                          new DigitalInput { DigitalInputId = -2, BuildingId = -1, ProtocolName = ProtocolNames.ShinyBoard, DeviceId = 0, InputName = "Low bat", Function = DigitalInputFunction.BeginShutdown, Inverted = false });
            });

            modelBuilder.Entity<DigitalOutput>(x =>
            {
                x.ToTable(nameof(DigitalOutput));
            });

            modelBuilder.Entity<Heater>(x =>
            {
                x.ToTable(nameof(Heater));

                x.HasOne<Building>().WithMany(b => b.Heaters).HasForeignKey(h => h.BuildingId);
                x.HasOne(h => h.DigitalOutput).WithOne().HasForeignKey<Heater>(h => h.DigitalOutputId).OnDelete(DeleteBehavior.Cascade);
                x.HasOne(h => h.Zone).WithMany(z => z.Heaters).HasForeignKey(h => h.ZoneId);
                x.HasOne(h => h.PowerZone).WithMany(pz => pz.Heaters).HasForeignKey(h => h.PowerZoneId);
            });

            modelBuilder.Entity<PowerZone>(x =>
            {
                x.ToTable(nameof(PowerZone));

                x.Property(z => z.SwitchDelayBetweenOutputsSeconds).ValueGeneratedNever().HasDefaultValue(2);

                x.HasOne<Building>().WithMany(b => b.PowerZones).HasForeignKey(pz => pz.BuildingId);
            });

            modelBuilder.Entity<ScheduleItem>(x =>
            {
                x.ToTable(nameof(ScheduleItem));

                const string daysOfWeekConversionSeparator = ",";

                x.Property(si => si.DaysOfWeek).IsRequired().HasConversion(si => si.Cast<int>().JoinWith(daysOfWeekConversionSeparator),
                        str => str.Split(daysOfWeekConversionSeparator, StringSplitOptions.RemoveEmptyEntries).Select(dow => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dow)).ToArray());

                x.Property(si => si.BeginTime).IsRequired();
                x.Property(si => si.EndTime).IsRequired();

                x.HasOne<Zone>().WithMany(z => z.Schedule).HasForeignKey(si => si.ZoneId);
            });

            modelBuilder.Entity<TemperatureControlledZone>(x =>
            {
                x.ToTable(nameof(TemperatureControlledZone));
            });

            modelBuilder.Entity<TemperatureSensor>(x =>
            {
                x.ToTable(nameof(TemperatureSensor));

                x.Property(ts => ts.Name).IsRequired();
                x.Property(ts => ts.ProtocolName).IsRequired();
                x.Property(ts => ts.InputDescriptor).IsRequired().ValueGeneratedNever();

                x.HasOne<Building>().WithMany(b => b.TemperatureSensors).HasForeignKey(ts => ts.BuildingId);
                x.HasMany(ts => ts.TemperatureControlledZones).WithOne(tcz => tcz.TemperatureSensor).HasForeignKey(tcz => tcz.TemperatureSensorId);
            });

            modelBuilder.Entity<User>(x =>
            {
                x.ToTable(nameof(User));

                x.Property(u => u.Login).IsRequired();
                x.Property(u => u.PasswordHash).IsRequired();

                x.HasOne(u => u.CreatedBy).WithMany().HasForeignKey(u => u.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
                x.HasOne(u => u.DisabledBy).WithMany().HasForeignKey(u => u.DisabledByUserId).OnDelete(DeleteBehavior.Restrict);

                x.HasIndex(u => new { u.Login, u.IsActive }).HasFilter($"[{nameof(User.IsActive)}] = 1").IsUnique();

                x.HasData(new User
                {
                    UserId = -1,
                    Login = "user",
                    PasswordHash = "testowy".CalculateHash(),
                    IsActive = true,
                    IsBrowseable = true,
                    CreatedDate = new DateTime(2019, 7, 10, 18, 4, 28, 876, DateTimeKind.Utc).AddTicks(8468)
                });
            });

            modelBuilder.Entity<UserPermission>(x =>
            {
                x.ToTable(nameof(UserPermission));

                x.HasKey(up => new { up.UserId, up.Permission });

                x.HasOne(up => up.User).WithMany(u => u.UserPermissions).HasForeignKey(up => up.UserId);

                x.HasData(new UserPermission { UserId = -1, Permission = Permission.Dashboard },
                          new UserPermission { UserId = -1, Permission = Permission.Dashboard_ZoneSettings },
                          new UserPermission { UserId = -1, Permission = Permission.Configuration_Zones },
                          new UserPermission { UserId = -1, Permission = Permission.Configuration_PowerZones },
                          new UserPermission { UserId = -1, Permission = Permission.Configuration_Devices },
                          new UserPermission { UserId = -1, Permission = Permission.Configuration_Users });
            });

            modelBuilder.Entity<Zone>(x =>
            {
                x.ToTable(nameof(Zone));

                x.Property(z => z.Name).IsRequired();

                x.HasOne<Building>().WithMany(b => b.Zones).HasForeignKey(z => z.BuildingId);
                x.HasOne(z => z.TemperatureControlledZone).WithOne().HasForeignKey<Zone>(z => z.TemperatureControlledZoneId).OnDelete(DeleteBehavior.Cascade);
                x.HasMany(z => z.Heaters).WithOne(h => h.Zone).HasForeignKey(h => h.ZoneId);
                x.HasMany(z => z.Schedule).WithOne().HasForeignKey(si => si.ZoneId);
            });

            modelBuilder.Entity<AuditLog>(x =>
            {
                x.ToTable(nameof(AuditLog));

                x.Property(al => al.KeyValues).HasConversion(al => Newtonsoft.Json.JsonConvert.SerializeObject(al),
                                                             str => Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(str));

                x.Property(al => al.OldValues).HasConversion(al => Newtonsoft.Json.JsonConvert.SerializeObject(al),
                                                             str => Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(str));

                x.Property(al => al.NewValues).HasConversion(al => Newtonsoft.Json.JsonConvert.SerializeObject(al),
                                                             str => Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(str));
            });
        }

        private IEnumerable<AuditLog> GetAuditLogs()
        {
            ChangeTracker.DetectChanges();

            var currentDate = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.Entity is Counter || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var auditLog = new AuditLog
                {
                    TableName = entry.Metadata.Relational().TableName,
                    EventTs = currentDate
                };

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditLog.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditLog.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditLog.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditLog.OldValues[propertyName] = property.OriginalValue;
                                auditLog.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }

                yield return auditLog;
            }
        }
    }
}
