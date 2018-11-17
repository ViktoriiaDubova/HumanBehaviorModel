using HBM.Web.Models;
using System.Data.Entity;

namespace HBM.Web.Contexts
{
    public class ArticleDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ArticleDbContext() : base("MainDB")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Course>().HasMany(c => c.Students)
            //.WithMany(s => s.Courses)
            //.Map(t => t.MapLeftKey("CourseId")
            //.MapRightKey("StudentId")
            //.ToTable("CourseStudent"));
        }
    }
}