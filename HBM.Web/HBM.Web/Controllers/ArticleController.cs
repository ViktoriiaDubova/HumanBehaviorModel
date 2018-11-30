using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private int articlesPerPage = 6;
        private ArticleDbContext db = new ArticleDbContext();

        public static object[] Ordering = new[]{
            new { Text = "by title", Value = "header"},
            new { Text = "by title desc", Value = "header_desc"},
            new { Text = "recent", Value = "date_desc" },
            new { Text = "older", Value = "date" }
        };

        [AllowAnonymous]
        public ActionResult Index(string sortOrder, string search, int? page)
        {
            ViewBag.SortOrder = sortOrder ?? "date_desc";
            ViewBag.Search = search;

            var articles = db.Articles.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                articles = articles.Where(a => a.Header.Contains(search));
            switch (sortOrder)
            {
                case "header":
                    articles = articles.OrderBy(a => a.Header);
                    break;
                case "header_desc":
                    articles = articles.OrderByDescending(a => a.Header);
                    break;
                case "date":
                    articles = articles.OrderBy(a => a.DatePost);
                    break;
                case "date_desc":
                default:
                    articles = articles.OrderByDescending(a => a.DatePost);
                    break;             
            }
            
            int pageNumber = (page ?? 1);
            var list = articles.ToList().ToPagedList(pageNumber, articlesPerPage);
            return View(list);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Tags(int? selected, int? page)
        {
            IEnumerable<Article> articles = null;
            if (selected != null)
            {
                var tag = await db.Tags.FindAsync(selected);
                if (tag != null)
                {
                    articles = tag.Articles;
                }
            }
            var model = new TagsPageViewModel()
            {
                Articles = articles?.ToList().ToPagedList(page ?? 1, articlesPerPage),
                SelectedTag = selected ?? -1, Tags = db.Tags.ToList()
            };

            return View(model);
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
            if (article == null)
                return HttpNotFound("Article not found");
            var model = new ArticleEditViewModel()
            {
                Id = article.Id,
                Content = article.Content,
                Description = article.Description,
                Header = article.Header,
                Tags = Tag.ToString(article.Tags),
                ImageUrl = article.Image?.Path
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

                Image img = null;
                if (model.ImageFile != null)
                {
                    string fileName = FileController.UploadFile(model.ImageFile, Server, "Images/Articles/");                    
                    img = new Image() { Path = $"~/Images/Articles/{fileName}" };
                    db.Images.Add(img);
                    await db.SaveChangesAsync();
                }

                Article article = new Article()
                {
                    Header = model.Header,
                    Description = model.Description,
                    Content = model.Content,
                    UserId = int.Parse(User.Identity.GetUserId()),
                    DatePost = DateTime.UtcNow
                };
                if (img != null)
                    article.ImageId = img.Id;

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

                Image img = null;
                if (model.ImageFile != null)
                {
                    if (article.ImageId != null)
                        FileController.ReplaceFile(model.ImageFile, Server, article.Image.Path);                    
                    else
                    {
                        string fileName = FileController.UploadFile(model.ImageFile, Server, "Images/Articles/");
                        img = new Image() { Path = $"~/Images/Articles/{fileName}" };
                        db.Images.Add(img);
                        await db.SaveChangesAsync();
                        article.ImageId = img.Id;
                    }                    
                }

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