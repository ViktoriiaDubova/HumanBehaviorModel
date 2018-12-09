using PagedList;
using System.Linq;
using System.Web.Mvc;
using HBM.Web.Models;
using HBM.Web.Filters;
using HBM.Web.Contexts;
using HBM.Web.ViewModels;
using HBM.Web.Extensions;
using System.Threading.Tasks;

namespace HBM.Web.Controllers
{
    [Authorize, Admin]
    public class ManageController : Controller
    {
        private readonly int EntitiesPerPage = 10;
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Users(int? page)
        {
            int pageNumber = (page ?? 1);
            var list = db.Users.ToList().ToPagedList(pageNumber, EntitiesPerPage);
            return View(list);
        }
        public ActionResult Tags()
        {
            return View(db.Tags.ToList());
        }

        #region Permissions
        public ActionResult Permissions()
        {
            return View(db.Permissions.ToList());
        }
        [HttpPost]
        public ActionResult CreatePermission(PermissionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Permissions", db.Permissions.ToList());
            }
            var permission = db.Permissions.FirstOrDefault(p => p.Key == model.Key);
            if (permission != null)
            {
                ModelState.AddModelError("Key", "The permission with the same key already exists");
                return View("Permissions", db.Permissions.ToList());
            }
            db.Permissions.Add(new Permission() { Key = model.Key });
            db.SaveChanges();
            return RedirectToAction("Permissions");
        }
        public ActionResult DeletePermission(int id)
        {
            var permission = db.Permissions.Find(id);
            if (permission == null)
                return HttpNotFound($"Can not find permission with ID {id}");
            if (permission.IsLocked)
                return RedirectToAction("Roles");

            db.Permissions.Remove(permission);
            db.SaveChanges();
            return RedirectToAction("Permissions");
        }

        public ActionResult AssignPermissions(int roleId, string error)
        {
            var role = db.UserRoles.Find(roleId);
            if (role == null)
                return HttpNotFound($"Role with ID {roleId} not found");

            var model = new RolePermissionsViewModel()
            {
                RoleId = role.Id,
                RoleKey = role.Key,
                Permissions = db.Permissions
                              .Select(p => new SelectListItem()
                              {
                                  Text = p.Key,
                                  Value = p.Id.ToString()
                              }),
                SelectedPermissions = role.Permissions.Select(p => p.Id)
            };
            if (error != null)
                ModelState.AddModelError("", error);
            return View(model);
        }
        [HttpPost]
        public ActionResult AssignPermissions(RolePermissionsViewModel model)
        {
            var role = db.UserRoles.Find(model.RoleId);
            if (role == null)
                return HttpNotFound($"Role with ID {model.RoleId} not found");
            
            role.Permissions.Intersection(
                model.SelectedPermissions.Select(id => db.Permissions.Find(id)), 
                (p1, p2) => p1.Key == p2.Key);

            if (role.Permissions.Any(p => p == null))
                return RedirectToAction("AssignPermissions", new { id=role.Id, error="Invalid permission ID got, retry request" });
             
            db.SaveChanges();
            return RedirectToAction("Roles");
        }
        #endregion

        #region Roles
        public ActionResult Roles()
        {
            return View(db.UserRoles.ToList());
        }
        [HttpPost]
        public ActionResult CreateRole(RoleCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Roles", db.UserRoles.ToList());
            var permission = db.UserRoles.FirstOrDefault(p => p.Key == model.Key);
            if (permission != null)
            {
                ModelState.AddModelError("Key", "The role with the same key already exists");
                return View("Roles", db.UserRoles.ToList());
            }
            db.UserRoles.Add(new UserRole() { Key = model.Key });
            db.SaveChanges();
            return RedirectToAction("Roles");
        }
        public ActionResult DeleteRole(int id)
        {
            var role = db.UserRoles.Find(id);
            if (role == null)
                return HttpNotFound($"Can not find role with ID {id}");
            if (role.IsLocked)
                return RedirectToAction("Roles");

            db.UserRoles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Roles");
        }

        public async Task<ActionResult> AssignRole(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound($"User with ID {id} not found");
            var roles = new SelectList(db.UserRoles, "Id", "Key", user.UserRoleId);
            var viewModel = new AssignRoleViewModel()
            {
                UserId = id,
                UserName = user.UserName,
                Roles = roles,
                SelectedRoleId = user.UserRoleId
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await db.Users.FindAsync(model.UserId);
            if (user == null)
                return HttpNotFound($"User not found with ID {model.UserId}");
            var role = await db.UserRoles.FindAsync(model.SelectedRoleId ?? 0);
            if (role == null)
            {
                ModelState.AddModelError("Roles", "No role was selected or invalid role ID sent");
                return View(model);
            }
            if (role.Key == UserRoleKey.Blocked.AsString() && user.UserRole.Key != role.Key)
            {
                user.UserStats.Rating += UserStats.RatingPerBan;
                user.UserStats.TimesBanned += 1;
            }
            user.UserRole = role;
            await db.SaveChangesAsync();
            return RedirectToAction("Users");
        }
        #endregion

        public async Task<ActionResult> RolePermissions(int id)
        {
            var role = await db.UserRoles.FindAsync(id);
            if (role == null)
                return HttpNotFound($"Can not find role with ID -- {id}");
            return View(role);
        }

        public ActionResult Articles(int? page)
        {
            int pageNumber = (page ?? 1);
            var list = db.Articles.ToList().ToPagedList(pageNumber, EntitiesPerPage);
            return View(list);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}