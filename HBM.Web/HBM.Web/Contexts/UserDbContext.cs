using HBM.Web.Models;
using System.Data.Entity;

namespace HBM.Web.Contexts
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserDbContext() : base("MainDB")
        {

        }
    }
}