using System;
using System.Collections.Generic;
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
        private readonly int ArticlesPerPage = 6;
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
            var list = articles.ToList().ToPagedList(pageNumber, ArticlesPerPage);
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
                Articles = articles?.ToList().ToPagedList(page ?? 1, ArticlesPerPage),
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
            var user = await db.Users.FindAsync(int.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                var activity = FindOrAddUserActivity(user, article);
                activity.Viewed = true;
                await db.SaveChangesAsync();
            }
            return View(article);
        }
        
        public ActionResult Create()
        {
            if (!HasPermission(PermissionKey.CreateArticle))
                return HttpNotFound("You are not allowed to create articles");
            return View();
        }
        
        public async Task<ActionResult> Edit(int id)
        {
            if (!HasPermission(PermissionKey.EditArticle))
                return HttpNotFound("You are not allowed to edit articles");
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
            if (!HasPermission(PermissionKey.CreateArticle))
                return HttpNotFound("You are not allowed to create articles");
            if (ModelState.IsValid)
            {
                var tagCreationResults = VerifyTags(model.Tags);
                if (!ModelState.IsValid)
                    return View(model);

                Image img = null;
                if (model.ImageFile != null)
                {
                    string fileName = FileController.UploadFile(model.ImageFile, Server, FileController.Paths["article_img"]);                    
                    img = new Image() { Path = $"~/{FileController.Paths["article_img"]}{fileName}" };
                    db.Images.Add(img);
                    await db.SaveChangesAsync();
                }

                var user = await db.Users.FindAsync(int.Parse(User.Identity.GetUserId()));
                Article article = new Article()
                {
                    Header = model.Header,
                    Description = model.Description,
                    Content = model.Content,
                    UserId = user.Id,
                    DatePost = DateTime.UtcNow
                };
                if (img != null)
                    article.ImageId = img.Id;

                var tagKeys = tagCreationResults.Select(r => r.Value);
                article.Tags = LoadTags(tagKeys).ToList();
                db.Articles.Add(article);

                user.UserStats.ArticlesPosted += 1;
                user.UserStats.Rating += UserStats.RatingPerArticle;

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReplyArticle(ReplyArticleViewModel model)
        {
            if (!HasPermission(PermissionKey.ReplyArticle))
                return HttpNotFound("You are not allowed to reply on articles");
            if (string.IsNullOrWhiteSpace(model.Text))
                ModelState.AddModelError("Text", "The text is empty!");
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Show", new { id = model.ArticleId });
            }

            var article = await db.Articles.FindAsync(model.ArticleId);
            if (article == null)
                return HttpNotFound("Article to reply was not found");
            var userId = int.Parse(User.Identity.GetUserId());
            var user = await db.Users.FindAsync(userId);
            var comment = new Comment()
            {
                Article = article,
                DatePost = DateTime.UtcNow,
                Text = model.Text.Trim(),
                User = user
            };
            article.Comments.Add(comment);

            user.UserStats.CommentsWritten += 1;
            user.UserStats.Rating += UserStats.RatingPerComment;

            await db.SaveChangesAsync();

            return RedirectToAction("Show", new { id= model.ArticleId });
        }

        public async Task<ActionResult> VoteUp(int id) => await Vote(id, 1);
        public async Task<ActionResult> VoteDown(int id) => await Vote(id, -1);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ArticleEditViewModel model)
        {
            if (!HasPermission(PermissionKey.EditArticle))
                return HttpNotFound("You are not allowed to edit articles");
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
                        string fileName = FileController.UploadFile(model.ImageFile, Server, FileController.Paths["article_img"]);
                        img = new Image() { Path = $"~/{FileController.Paths["article_img"]}{fileName}" };
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
            if (!HasPermission(PermissionKey.DeleteArticle))
                return HttpNotFound("You are not allowed to delete articles");
            var article = await db.Articles.FindAsync(id);
            if (article == null)
                return HttpNotFound("Article to delete not found");
            if (article.ImageId != null)
                FileController.RemoveFile(Server, article.Image.Path);
            db.Articles.Remove(article);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private async Task<ActionResult> Vote(int id, int value)
        {
            if (value != 1 && value != -1)
                throw new Exception("Unexpected voting behavior occured");
            var article = await db.Articles.FindAsync(id);
            if (article == null)
                return HttpNotFound("The requested article is not found");
            var user = GetCurrentUser();
            if (user.Id == article.UserId)
                return RedirectToAction("Show", new { id });

            var activity = FindOrAddUserActivity(user, article);
            if (value == -1 && activity.Vote > -1 || value == 1 && activity.Vote < 1)
            {
                activity.Vote += value;
                article.User.UserStats.Rating += UserStats.RatingPerArticleVote * value;
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Show", new { id });
        }

        private bool HasPermission(PermissionKey key)
        {
            var user = GetCurrentUser();
            return user != null && user.HasPermission(key);
        }
        private bool HasPermission(string key)
        {
            var user = GetCurrentUser();
            return user != null && user.HasPermission(key);
        }
        private ApplicationUser GetCurrentUser() => GetUser(User.Identity?.GetUserId());
        private ApplicationUser GetUser(int id) => db.Users.Find(id);
        private ApplicationUser GetUser(string id) => id != null ? GetUser(int.Parse(id)) : null;
        private UserArticleActivity FindOrAddUserActivity(ApplicationUser user, Article article)
        {
            var activity = db.UserArticleActivities.FirstOrDefault(a => a.UserId == user.Id && a.ArticleId == article.Id);
            if (activity == null)
            {
                activity = new UserArticleActivity()
                {
                    Article = article,
                    User = user,
                    Vote = 0
                };
                db.UserArticleActivities.Add(activity);
            }
            return activity;
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