using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required, StringLength(32, MinimumLength = 3)]
        public string UserIdent { get; set; }

        [Required(ErrorMessage = "Please, enter your password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}