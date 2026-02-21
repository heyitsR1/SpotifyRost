using Microsoft.EntityFrameworkCore;
using SpotifyRoast.Models;

namespace SpotifyRoast.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuRole> MenuRoles { get; set; }
        public DbSet<RoastPersonality> RoastPersonalities { get; set; }
        public DbSet<Roast> Roasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserRole Composite Key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configure MenuRole Composite Key
            modelBuilder.Entity<MenuRole>()
                .HasKey(mr => new { mr.RoleId, mr.MenuId });

            modelBuilder.Entity<MenuRole>()
                .HasOne(mr => mr.Role)
                .WithMany(r => r.MenuRoles)
                .HasForeignKey(mr => mr.RoleId);

            modelBuilder.Entity<MenuRole>()
                .HasOne(mr => mr.Menu)
                .WithMany(m => m.MenuRoles)
                .HasForeignKey(mr => mr.MenuId);

            // Global Query Filters (Soft Delete)
            modelBuilder.Entity<RoastPersonality>().HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
