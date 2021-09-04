using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Supermarket.Models;
using Supermarket.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Supermarket.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
    
        public DbSet<Store> Stores { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Kind> Kinds { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderDetailHistory> OrderDetailHistories { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.ProductId });

                entity.ToTable("Cart");

                entity.Property(e => e.AccountId).HasColumnName("AccountId");

                entity.Property(e => e.ProductId).HasColumnName("ProductId");

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Account");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Product");
            });
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<EntityEntry> modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
            foreach (EntityEntry item in modified)
            {
                if (item.Entity is IDateTracking changedOrAddedItem)
                {
                    if (item.State == EntityState.Added)
                    {
                        changedOrAddedItem.CreatedTime = DateTime.Now;
                    }
                    else
                    {
                        changedOrAddedItem.ModifiedTime = DateTime.Now;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.LogTo(Console.WriteLine);
    }
}
