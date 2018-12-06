using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HBM.Web.Contexts;
using HBM.Web.Filters;
using HBM.Web.ViewModels;
using PagedList;

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

        public ActionResult Permissions()
        {
            return View(db.Permissions.ToList());
        }

        public ActionResult Roles()
        {
            return View(db.UserRoles.ToList());
        }

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

        public async Task<ActionResult> AssignRole(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound($"User with ID {id} not found");
            var roles = new SelectList(db.UserRoles, "Id", "Key", user.UserRoleId);
            var viewModel = new AssignRoleViewModel()
            {
                UserId = id, UserName = user.UserName,
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
            user.UserRole = role;
            await db.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}