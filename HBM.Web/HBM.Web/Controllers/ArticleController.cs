using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using HBM.Web.Contexts;
using HBM.Web.Extensions;
using HBM.Web.Models;
using HBM.Web.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;

namespace HBM.Web.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private int articlesPerPage = 3;
        private ArticleDbContext db = new ArticleDbContext();

        // GET: Article
        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            var list = db.Articles.ToList();
            return View(list.ToPagedList(pageNumber, articlesPerPage));
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
        public async Task<ActionResult> Edit(int id)
        {
            var article = await db.Articles.FindAsync(id);
            var model = new ArticleEditViewModel()
            {
                Id = article.Id,
                Content = article.Content,
                Description = article.Description,
                Header = article.Header,
                Tags = Tag.ToString(article.Tags)
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ArticleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tagCreationResults = VerifyTags(model.Tags);
                if (!ModelState.IsValid)
                    return View(model);

                Article article = new Article()
                {
                    Header = model.Header,
                    Description = model.Description,
                    Content = model.Content,
                    UserId = int.Parse(User.Identity.GetUserId()),
                    DatePost = DateTime.UtcNow
                };
                var tagKeys = tagCreationResults.Select(r => r.Value);
                article.Tags = LoadTags(tagKeys).ToList();
                db.Articles.Add(article);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ArticleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = await db.Articles.FindAsync(model.Id);
                if (article == null)
                {
                    ModelState.AddModelError("", "Invalid article ID given");
                    return View(model);
                }
                int userId = int.Parse(User.Identity.GetUserId());
                if (article.UserId != userId)
                {
                    ModelState.AddModelError("", "You are not allowed to change the article");
                    return View(model);
                }
                var tagCreationResults = VerifyTags(model.Tags);
                if (!ModelState.IsValid)
                    return View(model);

                article.LoadFrom(model);
                var tagKeys = tagCreationResults.Select(r => r.Value);
                article.Tags.Intersection(LoadTags(tagKeys).ToList(), (t1, t2) => t1.Key == t2.Key);
                article.DateEdited = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return RedirectToAction("Show", new { id = article.Id });
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var article = await db.Articles.FindAsync(id);
            if (article == null)
                return HttpNotFound("Article to delete not found");
            db.Articles.Remove(article);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private IEnumerable<Tag> LoadTags(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key == key);
                if (tag == null)
                {
                    tag = new Tag() { Key = key };
                    db.Tags.Add(tag);
                }
                yield return tag;
            }
        }
        private IEnumerable<TagCreationResult> VerifyTags(string tagsLine)
        {
            var tagCreationResults = Tag.CreateTags(tagsLine);
            foreach (var result in tagCreationResults)
            {
                if (result.ErrorMessage != null)
                    ModelState.AddModelError("Tags", result.ErrorMessage);
            }
            return tagCreationResults;
        }
    }
}