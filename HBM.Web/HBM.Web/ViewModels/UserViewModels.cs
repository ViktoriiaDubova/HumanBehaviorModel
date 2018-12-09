using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class UserShowViewModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public int Articles { get; set; }
        public int Comments { get; set; }
        public int Banned { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public string Avatar { get; set; }
    }

    public class UserEditViewModel
    {
        [Required]
        public int Id { get; set; }
        public string UserName { get; set; }
        [StringLength(256)]
        public string About { get; set; }
        [StringLength(64), RegularExpression(@"^[\p{L}]+[\s]?[\p{L}]*$")]
        public string FullName { get; set; }
        public string ImageUrl { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageFile { get; set; }
    }

    public class LoginViewModel
    {
        [Required, StringLength(32, MinimumLength = 3)]
        public string UserIdent { get; set; }

        [Required(ErrorMessage = "Please, enter your password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

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