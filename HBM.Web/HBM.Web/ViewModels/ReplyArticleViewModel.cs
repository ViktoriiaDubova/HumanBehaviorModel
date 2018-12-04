using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class ReplyArticleViewModel
    {
        [Required]
        public int ArticleId { get; set; }
        [Required]
        [StringLength(521, MinimumLength = 2, ErrorMessage = "Please, make sure your message is less than 512 symbols and longer than 2")]
        public string Text { get; set; }
    }
}