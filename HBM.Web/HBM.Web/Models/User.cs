using System.ComponentModel.DataAnnotations;

namespace HBM.Web.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}