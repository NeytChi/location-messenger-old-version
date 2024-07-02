using System;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using miniMessanger.Models;

#nullable disable

namespace miniMessanger.Models
{
    /// <summary>
    /// Контект БД для управления таблицами данных.
    /// </summary>
    public partial class Context : DbContext
    {
        public bool manualControl = false;
        public bool useInMemoryDatabase = false;
        public Context()
        {
        }
        public Context(bool manualControl)
        {
            this.manualControl = manualControl;
        }
        public Context(bool manualControl, bool useInMemoryDatabase)
        {
            this.manualControl = manualControl;
            this.useInMemoryDatabase = useInMemoryDatabase;
        }
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<BlockedUser> BlockedUsers { get; set; }
        public virtual DbSet<Chatroom> Chatroom { get; set; }
        public virtual DbSet<Complaint> Complaint { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Participants> Participants { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<LikeProfiles> LikeProfile { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (useInMemoryDatabase)
            {
                optionsBuilder.UseInMemoryDatabase("messanger");
            }
            optionsBuilder.EnableSensitiveDataLogging();
            if (manualControl)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    Config config = new();
                    optionsBuilder.UseMySql(config.GetDatabaseConfigConnection(), ServerVersion.Parse("8.0.24-mysql"));
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlockedUser>(entity =>
            {
                entity.HasKey(block => block.BlockedId)
                    .HasName("PRIMARY");

                entity.ToTable("blocked_users");

                entity.HasIndex(block => block.UserId)
                    .HasName("user_id");

                entity.Property(block => block.BlockedId)
                    .HasColumnName("blocked_id")
                    .HasColumnType("int(11)");

                entity.Property(block => block.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.Property(block => block.BlockedUserId)
                    .HasColumnName("blocked_user_id")
                    .HasColumnType("int(11)");

                entity.Property(block => block.BlockedDeleted)
                    .HasColumnName("blocked_deleted")
                    .HasColumnType("boolean");

                entity.Property(block => block.BlockedReason)
                    .HasColumnName("blocked_reason")
                    .HasColumnType("varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci");

                entity.HasOne(block => block.User)
                    .WithMany(user => user.Blocks)
                    .HasForeignKey(block => block.UserId)
                    .HasConstraintName("blocked_users_ibfk_1");
            });
        }
    }
}