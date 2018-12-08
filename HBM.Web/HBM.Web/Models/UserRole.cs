using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HBM.Web.Models
{
    public enum UserRoleKey
    {
        Admin,
        Common,
        Moderator,
        Blocked,
        Unauthorized
    }
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }

        public bool HasPermission(string key) => Permissions?.FirstOrDefault(p => p.Key == key) != null;
    }

    public static class UserRoleHelpers
    {
        public static string AsString(this UserRoleKey key) => key.ToString().ToLower();
    }
}