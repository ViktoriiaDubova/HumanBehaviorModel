using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using HBM.Web.ViewModels;

namespace HBM.Web.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [StringLength(64, MinimumLength = 8)]
        public string Header { get; set; }
        [AllowHtml]
        [StringLength(256, MinimumLength = 24)]
        public string Description { get; set; }
        [AllowHtml]
        [StringLength(35000, MinimumLength = 64)]
        public string Content { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Image")]
        public int? ImageId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DatePost { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateEdited { get; set; }

        public virtual Image Image { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<UserArticleActivity> UserArticleActivities { get; set; }

        public void LoadFrom(ArticleEditViewModel model)
        {
            Header = model.Header;
            Description = model.Description;
            Content = model.Content;
        }
    }
}