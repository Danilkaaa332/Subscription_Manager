using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SubscriptionManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=subscriptions.db").LogTo(message => Debug.WriteLine(message), LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceName).IsRequired().HasDefaultValue("");
                entity.Property(e => e.Category).HasDefaultValue("Другое");
                entity.Property(e => e.Cost).IsRequired();
                entity.Property(e => e.PaymentPeriod).HasDefaultValue("Ежемесячно");
                entity.Property(e => e.NextBillingDate).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue("Активна");
                entity.Property(e => e.ManagementUrl).HasDefaultValue("");
                entity.Property(e => e.AddedDate).IsRequired();
                entity.Property(e => e.Notes).HasDefaultValue("");
            });
        }
    }
}