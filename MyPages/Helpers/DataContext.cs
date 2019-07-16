using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MyPages.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Helpers
{
    public class DataContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Page> Pages { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }

        protected DataContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<User>(entity =>
                {
                    entity.HasKey(u => u.Id);
                    entity.Property(u => u.Username).IsRequired();
                    entity.HasIndex(u => u.Username).IsUnique();
                    entity.HasOne(u => u.MainPage)
                    .WithOne()
                    .HasForeignKey<User>(u => u.MainPageId);
                })
                .Entity<Page>(entity =>
                {
                    entity.HasKey(p => p.Id);
                    entity.Property(p => p.Name).IsRequired();
                    entity.HasOne(p => p.Parent)
                        .WithMany(f => f.Children)
                        .HasForeignKey(p => p.ParentId);
                });
        }
    }
}
