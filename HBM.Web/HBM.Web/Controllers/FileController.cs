using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HBM.Web.Controllers
{
    public static class FileController
    {
        public static void ReplaceFile(HttpPostedFileBase file, HttpServerUtilityBase server, string destination)
        {
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
            string filePath = Path.Combine(server.MapPath($"~/{destination}"), fileName);
            file.SaveAs(filePath);
            return fileName;
        }        
    }
}