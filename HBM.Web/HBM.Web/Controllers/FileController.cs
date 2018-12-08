using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;

namespace HBM.Web.Controllers
{
    public static class FileController
    {
        public static ReadOnlyDictionary<string, string> Paths = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            ["images"] = "Uploads/Images/",
            ["article_img"] = "Uploads/Images/Articles/",
            ["user_img"] = "Uploads/Images/Users/"
        });

        public static void ReplaceFile(HttpPostedFileBase file, HttpServerUtilityBase server, string destination)
        {
            var directory = Path.GetDirectoryName(destination);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            file.SaveAs(server.MapPath(destination));
        }
        public static void RemoveFile(HttpServerUtilityBase server, string fileName)
        {
            string path = server.MapPath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static string UploadFile(HttpPostedFileBase file, HttpServerUtilityBase server, string destination)
        {
            string imageExtension = Path.GetExtension(file.FileName);
            string fileName = DateTime.UtcNow.ToString("yymmssfff") + imageExtension;
            string directory = server.MapPath($"~/{destination}");
            string filePath = Path.Combine(directory, fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            file.SaveAs(filePath);
            return fileName;
        }        
    }
}