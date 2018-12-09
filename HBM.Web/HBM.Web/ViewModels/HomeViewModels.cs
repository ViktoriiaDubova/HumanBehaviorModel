using System.Web.Mvc;

namespace HBM.Web.ViewModels
{
    public class PageEditViewModel
    {
        public string Page { get; set; }
        [AllowHtml]
        public string Html { get; set; }
    }
}
