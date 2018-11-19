using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class ArticleCreateViewModel
    {
        [Required, StringLength(64, MinimumLength = 8, ErrorMessage = "Article title is supposed to be 8 to 64 characters length")]
        public string Header { get; set; }
        [AllowHtml, UIHint("tinymce_full")]
        [Required, StringLength(256, MinimumLength = 24, ErrorMessage = "Article title is supposed to be 24 to 256 characters length")]
        public string Description { get; set; }
        [AllowHtml, UIHint("tinymce_full")]
        [Required, StringLength(6000, MinimumLength = 64, ErrorMessage = "Article title is supposed to be 64 to 6000 characters length")]
        public string Content { get; set; }
        [StringLength(64)]
        public string Tags { get; set; }
    }
}