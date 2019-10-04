using Commons.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Storage.StorageDatabase
{
    public class EfContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Domain.Building> Buildings { get; set; }
        public DbSet<Domain.Counter> Counters { get; set; }
        public DbSet<Domain.DigitalInput> DigitalInputs { get; set; }
        public DbSet<Domain.DigitalOutput> DigitalOutputs { get; set; }
        public DbSet<Domain.Heater> Heaters { get; set; }
        public DbSet<Domain.PowerZone> PowerZones { get; set; }
        public DbSet<Domain.TemperatureSensor> TemperatureSensors { get; set; }
        public DbSet<Domain.User> Users { get; set; }
        public DbSet<Domain.Zone> Zones { get; set; }

        public EfContext()
        {
            _connectionString = "Filename=test.db";
        }

        public EfContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Building>(x =>
            {
                x.ToTable(nameof(Domain.Building));

                x.Property(b => b.Name).IsRequired();
                x.Property(b => b.IsDefault).IsRequired();
                x.Property(b => b.ControlLoopIntervalSecondsMilliseconds).IsRequired();

                x.HasData(new Domain.Building
                {
                    BuildingId = -1,
                    ControlLoopIntervalSecondsMilliseconds = 5000,
                    IsDefault = true,
                    Name = "Budynek testowy"
                });
            });

            modelBuilder.Entity<Domain.Counter>(x =>
            {
                x.ToTable(nameof(Domain.Counter));

                x.HasOne(c => c.ResettedBy).WithMany().HasForeignKey(c => c.ResettedByUserId).OnDelete(DeleteBehavior.Restrict);

                x.HasIndex(c => new { c.HeaterId, c.ResetDate }).HasFilter($"[{nameof(Domain.Counter.ResetDate)}] IS NULL").IsUnique();
            });

            modelBuilder.Entity<Domain.DigitalInput>(x =>
            {
                x.ToTable(nameof(Domain.DigitalInput));

                x.HasOne<Domain.Building>().WithMany(b => b.DigitalInputs).HasForeignKey(di => di.BuildingId);
            });

            modelBuilder.Entity<Domain.DigitalOutput>(x =>
            {
                x.ToTable(nameof(Domain.DigitalOutput));
            });

            modelBuilder.Entity<Domain.Heater>(x =>
            {
                x.ToTable(nameof(Domain.Heater));

                x.HasOne<Domain.Building>().WithMany(b => b.Heaters).HasForeignKey(h => h.BuildingId);
                x.HasOne(h => h.DigitalOutput).WithOne().HasForeignKey<Domain.Heater>(h => h.DigitalOutputId).OnDelete(DeleteBehavior.Cascade);
                x.HasOne(h => h.Zone).WithMany(z => z.Heaters).HasForeignKey(h => h.ZoneId);
                x.HasOne(h => h.PowerZone).WithMany(pz => pz.Heaters).HasForeignKey(h => h.PowerZoneId);
            });

            modelBuilder.Entity<Domain.PowerZone>(x =>
            {
                x.ToTable(nameof(Domain.PowerZone));

                x.HasOne<Domain.Building>().WithMany(b => b.PowerZones).HasForeignKey(pz => pz.BuildingId);
            });

            modelBuilder.Entity<Domain.ScheduleItem>(x =>
            {
                x.ToTable(nameof(Domain.ScheduleItem));

                const string daysOfWeekConversionSeparator = ",";

                x.Property(si => si.DaysOfWeek).IsRequired().HasConversion(si => si.Cast<int>().JoinWith(daysOfWeekConversionSeparator),
                    str => str.Split(daysOfWeekConversionSeparator, StringSplitOptions.RemoveEmptyEntries).Select(dow=>(DayOfWeek)Enum.Parse(typeof(DayOfWeek), dow)).ToArray());

                x.Property(si => si.BeginTime).IsRequired();
                x.Property(si => si.EndTime).IsRequired();

                x.HasOne<Domain.Zone>().WithMany(z => z.Schedule).HasForeignKey(si => si.ZoneId);
            });

            modelBuilder.Entity<Domain.TemperatureControlledZone>(x =>
            {
                x.ToTable(nameof(Domain.TemperatureControlledZone));
            });

            modelBuilder.Entity<Domain.TemperatureSensor>(x =>
            {
                x.ToTable(nameof(Domain.TemperatureSensor));

                x.Property(ts => ts.Name).IsRequired();
                x.Property(ts => ts.DeviceId).IsRequired();

                x.HasOne<Domain.Building>().WithMany(b => b.TemperatureSensors).HasForeignKey(ts => ts.BuildingId);
                x.HasMany(ts => ts.TemperatureControlledZones).WithOne(tcz => tcz.TemperatureSensor).HasForeignKey(tcz => tcz.TemperatureSensorId);
            });

            modelBuilder.Entity<Domain.User>(x =>
            {
                x.ToTable(nameof(Domain.User));

                x.Property(u => u.Login).IsRequired();
                x.Property(u => u.PasswordHash).IsRequired();

                x.HasOne(u => u.CreatedBy).WithMany().HasForeignKey(u => u.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
                x.HasOne(u => u.DisabledBy).WithMany().HasForeignKey(u => u.DisabledByUserId).OnDelete(DeleteBehavior.Restrict);

                x.HasIndex(u => new { u.Login, u.IsActive }).HasFilter($"[{nameof(Domain.User.IsActive)}] = 1").IsUnique();

                x.HasData(new Domain.User
                {
                    UserId = -1,
                    Login = "user",
                    PasswordHash = "testowy".CalculateHash(),
                    IsActive = true,
                    IsBrowseable = true,
                    CreatedDate = new DateTime(2019, 7, 10, 18, 4, 28, 876, DateTimeKind.Utc).AddTicks(8468)
                });
            });

            modelBuilder.Entity<Domain.Zone>(x =>
            {
                x.ToTable(nameof(Domain.Zone));

                x.Property(z => z.Name).IsRequired();

                x.HasOne<Domain.Building>().WithMany(b => b.Zones).HasForeignKey(z => z.BuildingId);
                x.HasOne(z => z.TemperatureControlledZone).WithOne().HasForeignKey<Domain.Zone>(z => z.TemperatureControlledZoneId).OnDelete(DeleteBehavior.Cascade);
                x.HasMany(z => z.Heaters).WithOne(h => h.Zone).HasForeignKey(h => h.ZoneId);
                x.HasMany(z => z.Schedule).WithOne().HasForeignKey(si => si.ZoneId);
            });
        }
    }
}
