using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HBM.Web.Contexts;
using HBM.Web.Models;

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

    public class UserManager : IDisposable
    {
        private static UserManager instance;
        public static UserManager Instance => instance ?? (instance = new UserManager());

        private UserDbContext dbContext;

        private UserManager ()
        {
            dbContext = new UserDbContext();
        }
        
        public async Task<UserCreationResult> CreateUser(string username, string email, string password)
        {
            if (dbContext.Users.Any(u => u.Username == username))
                return UserCreationResult.UsernameDuplicate;
            if (dbContext.Users.Any(u => u.Email == email))
                return UserCreationResult.UserEmailDuplicate;

            ApplicationUser user = new ApplicationUser()
            {
                Username = username, Email = email,
                IsEmailConfirmed = false, 
                PasswordHash = HashPassword(password)
            };
            dbContext.Users.Add(user);
            int result = 0;
            try
            {
                result = await dbContext.SaveChangesAsync();
            }
            catch { }
            if (result == 0)
                return UserCreationResult.UnknownError;
            return UserCreationResult.Created;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        private bool VerifyPassword(string password, string hash)
        {
            string phash = HashPassword(password);
            return password == phash;
        }
        private string HashPassword(string password)
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
    }
}