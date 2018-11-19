using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HBM.Web.Managers;
using HBM.Web.Models;
using HBM.Web.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace HBM.Web.Controllers
{
    public class UserController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public UserController()
        {

        }
        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? (_signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>());
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());
            private set => _userManager = value;
        }

        // GET: User
        [Authorize]
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
        [Authorize]
        public ActionResult SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                AuthenticationManager.SignOut();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByNameOrEmailAsync(model.UserIdent, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Incorrect username/email or password.");
                }
                else
                {
                    ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    claim.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String));
                    claim.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName, ClaimValueTypes.String));
                    claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "OWIN Provider", ClaimValueTypes.String));
                    //if (user.Role != null)
                    //    claim.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name, ClaimValueTypes.String));

                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

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