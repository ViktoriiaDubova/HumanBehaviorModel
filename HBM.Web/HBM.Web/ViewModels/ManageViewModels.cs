using System.Web.Mvc;
using HBM.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class AssignRoleViewModel
    {
        public int UserId { get; set; }
        public int? SelectedRoleId { get; set; }
        public string UserName { get; set; }
        public SelectList Roles { get; set; }
    }
    
    public class PermissionCreateViewModel
    {
        [Required]
        [StringLength(24), RegularExpression(@"^\w+$", ErrorMessage = "The key must contain only letters, numbers and underscores")]
        public string Key { get; set; }
    }

    public class RoleCreateViewModel
    {
        [Required]
        [StringLength(16), RegularExpression(@"^\w+$", ErrorMessage = "The key must contain only letters, numbers and underscores")]
        public string Key { get; set; }
    }

    public class RolePermissionsViewModel
    {
        public int RoleId { get; set; }
        public string RoleKey { get; set; }
        public IEnumerable<SelectListItem> Permissions { get; set; }
        public IEnumerable<int> SelectedPermissions { get; set; }
    }

    public class TagEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 2), RegularExpression(Tag.Pattern, ErrorMessage = "Only letters, numbers and underscores are valid")]
        public string Key { get; set; }
    }
}