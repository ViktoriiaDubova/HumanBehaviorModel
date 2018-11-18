using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required, StringLength(32, MinimumLength = 3)]
        public string Username { get; set; }

        [EmailAddress, Required]
        [StringLength(32, MinimumLength = 3)]
        public string Email { get; set; }

        [Required, Compare("PasswordConfirmation", ErrorMessage = "Please, confirm your password")]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirmation { get; set; }
    }
}