using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBM.Web.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(32, MinimumLength = 3)]
        [Required, Index("IX_UserIdent", Order = 1, IsUnique = true)]
        public string Username { get; set; }

        [EmailAddress, Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(32, MinimumLength = 3)]
        [Index("IX_UserIdent", Order = 2, IsUnique = true)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        [ForeignKey("Avatar")]
        public int? ImageId { get; set; }

        public virtual Image Avatar { get; set; }
    }
}