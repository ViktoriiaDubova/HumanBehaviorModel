using HBM.Web.Models;
using System.Data.Entity;

namespace HBM.Web.Contexts
{
    /// <summary>
    /// The context used only to let EF know what models are in use
    /// </summary>
    public class ApplicationDbContext: DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public ApplicationDbContext() : base("MainDB")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>()
            .HasMany(a => a.Tags)
            .WithMany(t => t.Articles)
            .Map(x =>
            {
                x.MapLeftKey("Article_Id");
                x.MapRightKey("Tag_Id");
                x.ToTable("ArticleTags");
            });

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