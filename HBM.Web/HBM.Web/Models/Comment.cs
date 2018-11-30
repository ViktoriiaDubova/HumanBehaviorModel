using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBM.Web.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [StringLength(512, MinimumLength = 8)]
        public string Text { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DatePost { get; set; }

        public virtual Article Article { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}