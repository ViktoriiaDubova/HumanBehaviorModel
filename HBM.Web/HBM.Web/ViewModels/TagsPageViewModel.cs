using System.Collections.Generic;
using HBM.Web.Models;
using PagedList;

namespace HBM.Web.ViewModels
{
    public class TagsPageViewModel
    {
        public int SelectedTag { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public List<Tag> Tags { get; set; }
    }
}