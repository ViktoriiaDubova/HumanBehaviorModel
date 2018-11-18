using System.Threading.Tasks;
using System.Web.Mvc;
using HBM.Web.Managers;
using HBM.Web.ViewModels;

namespace HBM.Web.Controllers
{
    public class UserController : Controller
    {
        public UserManager UserManager => UserManager.Instance;

        // GET: User
        public ActionResult Index()
        {
            return HttpNotFound();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        //
        //public async Task<ActionResult> Login()
        //{

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {            
            if (ModelState.IsValid)
            {                
                var result = await UserManager.CreateUser(model.Username, model.Email, model.Password);
                if (result == UserCreationResult.Created)
                    return View("Login");
                if (result.HasFlag(UserCreationResult.UserEmailDuplicate))
                    ModelState.AddModelError("Email", "User with such an email is already exists");
                if (result.HasFlag(UserCreationResult.UsernameDuplicate))
                    ModelState.AddModelError("Username", "User with such a name is already exists");
                if (result.HasFlag(UserCreationResult.UnknownError))
                    ModelState.AddModelError("", "Invalid register attempt");
            }
            return View(model);
        }
    }
}