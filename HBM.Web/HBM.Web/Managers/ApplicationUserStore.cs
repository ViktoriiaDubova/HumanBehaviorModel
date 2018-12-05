using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HBM.Web.Contexts;
using HBM.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HBM.Web.Managers
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>
    {
        private UserStore<IdentityUser> userStore;

        public UserDbContext DbContext { get; }

        public ApplicationUserStore(UserDbContext context)
        {
            userStore = new UserStore<IdentityUser>(context);
            DbContext = context;
        }
        public Task CreateAsync(ApplicationUser user)
        {
            var context = userStore.Context as UserDbContext;
            context.Users.Add(user);
            context.Configuration.ValidateOnSaveEnabled = false;
            return context.SaveChangesAsync();
        }
        public Task DeleteAsync(ApplicationUser user)
        {
            var context = userStore.Context as UserDbContext;
            context.Users.Remove(user);
            context.Configuration.ValidateOnSaveEnabled = false;
            return context.SaveChangesAsync();
        }
        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var context = userStore.Context as UserDbContext;
            return context.Users.Where(u => u.Id.ToString() == userId).FirstOrDefaultAsync();
        }
        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var context = userStore.Context as UserDbContext;
            return await context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
        }
        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var context = userStore.Context as UserDbContext;
            return context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }
        public async Task<ApplicationUser> FindByUsernameOrEmailAsync(string userIdent)
        {
            var context = userStore.Context as UserDbContext;
            return await context.Users.Where(u => u.UserName == userIdent || u.Email == userIdent).FirstOrDefaultAsync();
        }
        public Task UpdateAsync(ApplicationUser user)
        {
            var context = userStore.Context as UserDbContext;
            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;
            context.Configuration.ValidateOnSaveEnabled = false;
            return context.SaveChangesAsync();
        }
        public void Dispose()
        {
            userStore.Dispose();
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            var identityUser = ToIdentityUser(user);
            var task = userStore.GetPasswordHashAsync(identityUser);
            SetApplicationUser(user, identityUser);
            return task;
        }
        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            var identityUser = ToIdentityUser(user);
            var task = userStore.HasPasswordAsync(identityUser);
            SetApplicationUser(user, identityUser);
            return task;
        }
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            var identityUser = ToIdentityUser(user);
            var task = userStore.SetPasswordHashAsync(identityUser, passwordHash);
            SetApplicationUser(user, identityUser);
            return task;
        }
        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            var identityUser = ToIdentityUser(user);
            var task = userStore.GetSecurityStampAsync(identityUser);
            SetApplicationUser(user, identityUser);
            return task;
        }
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            var identityUser = ToIdentityUser(user);
            var task = userStore.SetSecurityStampAsync(identityUser, stamp);
            SetApplicationUser(user, identityUser);
            return task;
        }
        private static void SetApplicationUser(ApplicationUser user, IdentityUser identityUser)
        {
            user.PasswordHash = identityUser.PasswordHash;
            user.SecurityStamp = identityUser.SecurityStamp;
            user.Id = int.Parse(identityUser.Id);
            user.UserName = identityUser.UserName;
        }
        private IdentityUser ToIdentityUser(ApplicationUser user)
        {
            return new IdentityUser
            {
                Id = user.Id.ToString(),
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                UserName = user.UserName
            };
        }
    }
}