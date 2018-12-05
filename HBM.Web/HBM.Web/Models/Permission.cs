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
    }

    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
    }
}