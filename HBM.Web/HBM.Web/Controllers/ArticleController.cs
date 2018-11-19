using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HBM.Web.Contexts;
using HBM.Web.Models;
using HBM.Web.ViewModels;
using Microsoft.AspNet.Identity;

namespace HBM.Web.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private ArticleDbContext db = new ArticleDbContext();

        // GET: Article
        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            var articles = db.Articles.ToList();
            return View(articles);
        }
        [AllowAnonymous]
        public async Task<ActionResult> Show(int id)
        {
            var article = await db.Articles.FindAsync(id);
            if (article == null)
                return HttpNotFound("Article not found!");
            return View(article);
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
        public async Task<ActionResult> Create(ArticleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Article article = new Article()
                {
                    Header = model.Header,
                    Description = model.Description,
                    Content = model.Content,
                    UserId = int.Parse(User.Identity.GetUserId())              
                };
                //TODO: add tags
                db.Articles.Add(article);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
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