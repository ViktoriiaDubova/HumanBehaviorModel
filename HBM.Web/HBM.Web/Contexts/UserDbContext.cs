using HBM.Web.Models;
using System.Data.Entity;

namespace HBM.Web.Contexts
{
    public class UserDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
        public DbSet<UserArticleActivity> UserArticleActivities { get; set; }

        public UserDbContext() : base("MainDB")
        {

        }

        public static UserDbContext Create() => new UserDbContext();

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>()
                .HasMany(r => r.Permissions)
                .WithMany().Map(x =>
                {
                    x.MapLeftKey("Role_Id");
                    x.MapRightKey("Permission_Id");
                    x.ToTable("RolePermissions");
                });
        }
    }
}