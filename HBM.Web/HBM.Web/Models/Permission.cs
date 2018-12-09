using System.ComponentModel.DataAnnotations;

namespace HBM.Web.Models
{
    public enum PermissionKey
    {
        CreateArticle,
        ReplyArticle,
        DeleteArticle,
        EditArticle,
        DeleteArticleReply,
        BlockUser,
        BlockArticle,
        LogIn,
        AssignRole,
        DeleteOtherUserArticle,
        EditHomePages
    }

    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public bool IsLocked { get; set; }
    }
}