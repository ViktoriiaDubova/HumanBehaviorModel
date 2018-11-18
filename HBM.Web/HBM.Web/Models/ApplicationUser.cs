using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public bool IsEmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        [ForeignKey("Avatar")]
        public int? ImageId { get; set; }

        public virtual Image Avatar { get; set; }
    }
}