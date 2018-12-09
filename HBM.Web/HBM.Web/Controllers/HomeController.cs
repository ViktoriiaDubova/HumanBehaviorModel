using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HBM.Web.Contexts;
using HBM.Web.Models;
using HBM.Web.ViewModels;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;

namespace HBM.Web.Controllers
{
    public class HomeController : Controller
    {
        private UserDbContext db;
        private UserDbContext DB => db ?? (db = new UserDbContext());

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public ActionResult Edit(string page)
        {            
            var user = DB.Users.Find(int.Parse(User.Identity.GetUserId()));
            if (!user.HasPermission(PermissionKey.EditHomePages))
                return HttpNotFound("You are not allowed to edit the Home pages");
            if (!HasPage(page))
                return HttpNotFound($"Page '{page}' not found");
            var model = new PageEditViewModel()
            {
                Page = page,
                Html = System.IO.File.ReadAllText(GetContentPath(page))
            };
            return View("PageEdit", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PageEditViewModel model)
        {
            var user = DB.Users.Find(int.Parse(User.Identity.GetUserId()));
            if (!user.HasPermission(PermissionKey.EditHomePages))
                return HttpNotFound("You are not allowed to edit the Index page");
            if (!HasPage(model.Page))
                return HttpNotFound($"Page 'model.Page' not found");
            System.IO.File.WriteAllText(GetContentPath(model.Page), model.Html);
            return RedirectToAction(model.Page);
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
                db.Dispose();
            base.Dispose(disposing);
        }
        
        private bool HasPage(string page)
        {
            string name = page.ToLower();
            return name == "index" || name == "about" || name == "contacts";
        }
        private string GetContentPath(string page)
        {
            return Server.MapPath($"~/Views/Home/Content/{page}.cshtml");
        }
    }
}