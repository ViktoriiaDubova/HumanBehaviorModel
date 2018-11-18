using HBM.Web.Models;
using System.Data.Entity;

namespace HBM.Web.Contexts
{
    public class ArticleDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public ArticleDbContext() : base("MainDB")
        {

        }
    }
}