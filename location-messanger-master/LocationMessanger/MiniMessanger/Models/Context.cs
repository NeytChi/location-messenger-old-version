using Microsoft.EntityFrameworkCore;

namespace miniMessanger.Models
{
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
        }
    }
}