using HBM.Web.Models;
using System.Data.Entity;

namespace HBM.Web.Contexts
{
    public class UserDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Image> Avatars { get; set; }

        public UserDbContext() : base("MainDB")
        {

        }

        public static UserDbContext Create()
        {
            return new UserDbContext();
        }
    }
}