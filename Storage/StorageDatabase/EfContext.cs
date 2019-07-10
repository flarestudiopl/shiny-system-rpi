using Commons.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Storage.StorageDatabase
{
    public class EfContext : DbContext
    {
        public DbSet<Domain.StorageDatabase.Counter> Counters { get; set; }

        public DbSet<Domain.StorageDatabase.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./ef_test.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.StorageDatabase.Counter>(x =>
            {
                x.ToTable(nameof(Domain.StorageDatabase.Counter));
                x.HasOne(c => c.ResettedBy).WithMany().HasForeignKey(c => c.ResettedByUserId).OnDelete(DeleteBehavior.Restrict);
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
                    CreatedDate = DateTime.UtcNow
                });
            });
        }
    }
}
