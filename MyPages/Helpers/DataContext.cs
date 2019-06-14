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
        public virtual DbSet<Folder> Folders { get; set; }
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
                })
                .Entity<Folder>(entity =>
                {
                    entity.HasKey(f => f.Id);
                    entity.Property(f => f.Name).IsRequired();
                    entity.HasOne(f => f.User)
                        .WithOne(u => u.Folder)
                        .HasForeignKey<Folder>(u => u.UserId);
                    entity.HasOne(f => f.Parent)
                        .WithMany(fp => fp.Childs)
                        .HasForeignKey(f => f.ParentId);
                })
                .Entity<Page>(entity =>
                {
                    entity.HasKey(p => p.Id);
                    entity.Property(p => p.Name).IsRequired();
                    entity.HasOne(p => p.Folder)
                        .WithMany(f => f.Pages)
                        .HasForeignKey(p => p.FolderId);
                });
        }
    }
}
