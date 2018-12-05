using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HBM.Web.Models
{
    public class ApplicationUser : IUser
    {
        [Key]
        public int Id { get; set; }
        string IUser<string>.Id => Id.ToString();

        [Column(TypeName = "VARCHAR")]
        [StringLength(32, MinimumLength = 3)]
        [Required, Index("IX_UserIdent", Order = 1, IsUnique = true)]
        public string UserName { get; set; }
        [EmailAddress, Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(32, MinimumLength = 3)]
        [Index("IX_UserIdent", Order = 2, IsUnique = true)]
        public string Email { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateRegistered { get; set; }

        [Required]
        public bool IsEmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        [ForeignKey("Avatar")]
        public int? ImageId { get; set; }
        public int? UserRoleId { get; set; }

        public virtual Image Avatar { get; set; }
        public virtual UserRole UserRole { get; set; }
        
        public bool HasPermission(string key)
        {
            if (UserRoleId == null || key == null)
                return false;
            return UserRole.HasPermission(key);
        }
        public bool HasPermission(PermissionKey key)
        {
            if (UserRoleId == null)
                return false;
            return UserRole.HasPermission(key.ToString());
        }
    }
}