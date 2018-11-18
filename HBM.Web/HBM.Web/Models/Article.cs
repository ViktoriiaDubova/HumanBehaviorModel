using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBM.Web.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [StringLength(64, MinimumLength = 8)]
        public string Header { get; set; }
        [StringLength(256, MinimumLength = 24)]
        public string Description { get; set; }
        [StringLength(6000, MinimumLength = 64)]
        public string Text { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Image")]
        public int? ImageId { get; set; }

        public virtual Image Image { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}