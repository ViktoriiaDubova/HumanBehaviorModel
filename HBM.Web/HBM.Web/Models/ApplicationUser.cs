using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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
        
        [StringLength(64, MinimumLength = 4)]
        public string FullName { get; set; }
        [StringLength(256, MinimumLength = 0)]
        public string About { get; set; }
        
        [Required]
        public bool IsEmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        [ForeignKey("Avatar")]
        public int? ImageId { get; set; }
        public int? UserRoleId { get; set; }
        [Required]
        public int UserStatsId { get; set; }

        public virtual Image Avatar { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual UserStats UserStats { get; set; }
        
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

    public class UserStats
    {
        public const int RatingPerBan = -30;
        public const int RatingPerComment = 3;
        public const int RatingPerArticle = 10;
        public const int RatingPerArticleVote = 3;

        [Required]
        [Key, ForeignKey("User")]
        [Index("IX_UserStats", IsUnique = true)]
        public int Id { get; set; }

        [Required]
        public int Rating { get; set; }
        [Required]
        public int TimesBanned { get; set; }
        [Required]
        public int ArticlesPosted { get; set; }
        [Required]
        public int CommentsWritten { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class UserArticleActivity
    {
        [Key]
        public int Id { get; set; }

        [Index("IX_UserArticle", Order = 1, IsUnique = true)]
        public int UserId { get; set; }

        [Index("IX_UserArticle", Order = 2, IsUnique = true)]
        public int ArticleId { get; set; }

        [Required]
        public bool Viewed { get; set; }
        [Required]
        public int Vote { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Article Article { get; set; }
    }
}