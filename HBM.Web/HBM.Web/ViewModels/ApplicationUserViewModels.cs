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
}