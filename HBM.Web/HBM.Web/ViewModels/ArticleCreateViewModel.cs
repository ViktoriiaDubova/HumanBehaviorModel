using System.ComponentModel.DataAnnotations;

namespace HBM.Web.ViewModels
{
    public class ArticleCreateViewModel
    {
        [Required, StringLength(64, MinimumLength = 8)]
        public string Header { get; set; }
        [Required, StringLength(256, MinimumLength = 24)]
        public string Description { get; set; }
        [Required, StringLength(6000, MinimumLength = 64)]
        public string Text { get; set; }
        [StringLength(64)]
        public string Tags { get; set; }
    }
}