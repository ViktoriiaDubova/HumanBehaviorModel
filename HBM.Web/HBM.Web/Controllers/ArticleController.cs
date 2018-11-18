using System;
using System.Linq;
using System.Web.Mvc;
using HBM.Web.Contexts;
using HBM.Web.ViewModels;

namespace HBM.Web.Controllers
{
    public class ArticleController : Controller
    {
        private ArticleDbContext db = new ArticleDbContext();

        // GET: Article
        public ActionResult Index()
        {
            var articles = db.Articles.ToList();
            return View(articles);
        }
        //Get: Create
        public ActionResult Create()
        {
            return View();
        }
        //Get: Edit
        public ActionResult Edit()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleCreateViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleEditViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}