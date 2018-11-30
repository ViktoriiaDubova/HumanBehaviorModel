using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class ArticleCreateViewModel
    {
        [Required, StringLength(64, MinimumLength = 8, ErrorMessage = "Article title is supposed to be 8 to 64 characters length")]
        public string Header { get; set; }
        [Required, StringLength(256, MinimumLength = 24, ErrorMessage = "Article description is supposed to be 24 to 256 characters length")]
        public string Description { get; set; }
        [AllowHtml, UIHint("tinymce_full")]
        [Required, StringLength(35000, MinimumLength = 64, ErrorMessage = "Article content is supposed to be 64 to 35000 characters long")]
        public string Content { get; set; }
        [StringLength(64)]
        public string Tags { get; set; }
    }
}