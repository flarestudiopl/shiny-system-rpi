﻿using Commons.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Storage.StorageDatabase
{
    public class EfContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Domain.StorageDatabase.Counter> Counters { get; set; }

        public DbSet<Domain.StorageDatabase.User> Users { get; set; }

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
            modelBuilder.Entity<Domain.StorageDatabase.Counter>(x =>
            {
                x.ToTable(nameof(Domain.StorageDatabase.Counter));
                
                x.HasOne(c => c.ResettedBy).WithMany().HasForeignKey(c => c.ResettedByUserId).OnDelete(DeleteBehavior.Restrict);

                x.HasIndex(c => new { c.HeaterId, c.ResetDate }).HasFilter($"[{nameof(Domain.StorageDatabase.Counter.ResetDate)}] IS NULL").IsUnique();
            });

            modelBuilder.Entity<Domain.StorageDatabase.User>(x =>
            {
                x.ToTable(nameof(Domain.StorageDatabase.User));

                x.Property(u => u.Login).IsRequired();
                x.Property(u => u.PasswordHash).IsRequired();

                x.HasOne(u => u.CreatedBy).WithMany().HasForeignKey(u => u.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
                x.HasOne(u => u.DisabledBy).WithMany().HasForeignKey(u => u.DisabledByUserId).OnDelete(DeleteBehavior.Restrict);

                x.HasIndex(u => new { u.Login, u.IsActive }).HasFilter($"[{nameof(Domain.StorageDatabase.User.IsActive)}] = 1").IsUnique();

                x.HasData(new Domain.StorageDatabase.User
                {
                    UserId = -1,
                    Login = "user",
                    PasswordHash = "testowy".CalculateHash(),
                    IsActive = true,
                    IsBrowseable = true,
                    CreatedDate = new DateTime(2019, 7, 10, 18, 4, 28, 876, DateTimeKind.Utc).AddTicks(8468)
                });
            });
        }
    }
}
