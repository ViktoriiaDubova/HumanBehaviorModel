using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HBM.Web.Contexts;
using HBM.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace HBM.Web.Managers
{
    [Flags]
    public enum UserCreationResult
    {
        Created = 0,
        UsernameDuplicate = 1,
        UserEmailDuplicate = 2,
        UnknownError = 4
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>, IDisposable
    {
        private ApplicationUserStore UserStore { get; }

        private ApplicationUserManager (ApplicationUserStore store) : base(store)
        {
            PasswordHasher = new InternalPasswordHasher();
            UserStore = store;
        }
        
        public async Task<UserCreationResult> CreateUser(string username, string email, string password)
        {
            if (await UserStore.FindByNameAsync(username) != null)
                return UserCreationResult.UsernameDuplicate;
            if (await UserStore.FindByEmailAsync(email) != null)
                return UserCreationResult.UserEmailDuplicate;

            ApplicationUser user = new ApplicationUser()
            {
                UserName = username,
                Email = email,
                IsEmailConfirmed = false,
                DateRegistered = DateTime.UtcNow,
                PasswordHash =  PasswordHasher.HashPassword(password)
            };
            int result = 1;
            try
            {
                await UserStore.CreateAsync(user);
            }
            catch (Exception ex)
            {
                result = 0;
            }
            if (result == 0)
                return UserCreationResult.UnknownError;
            return UserCreationResult.Created;
        }
       
        public override async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var user = await UserStore.FindByNameAsync(userName);
            var result = PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
                return user;
            return null;
        }

        public async Task<ApplicationUser> FindByNameOrEmailAsync(string userIdent, string password)
        {
            var user = await UserStore.FindByUsernameOrEmailAsync(userIdent);
            var result = PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
                return user;
            return null;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<UserDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            //manager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

    }
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public async Task<SignInStatus> PasswordSignInAsync(ApplicationUserManager userManager, string userIdent, string password, bool persistent, bool shouldLockout)
        {
            var user = await userManager.FindByNameOrEmailAsync(userIdent, password);
            if (user == null)
                return SignInStatus.Failure;
            if (!user.IsEmailConfirmed)
                return SignInStatus.RequiresVerification;
            return await PasswordSignInAsync(user.UserName, password, persistent, shouldLockout);
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
            //return (ApplicationUserManager)UserManager.CreateIdentityAsync(user, string.Empty);
            //return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    internal class InternalPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            byte[] hashData = null;
            using (MD5 md5 = MD5.Create())
            {
                hashData = md5.ComputeHash(Encoding.Default.GetBytes(password));
            }
            StringBuilder returnValue = new StringBuilder();
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }
            return returnValue.ToString();
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            string phash = HashPassword(providedPassword);
            return hashedPassword == phash ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}