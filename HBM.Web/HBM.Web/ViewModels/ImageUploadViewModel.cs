using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HBM.Web.ViewModels
{
    public class ImageUploadViewModel
    {
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}