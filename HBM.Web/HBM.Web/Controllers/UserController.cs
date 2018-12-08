using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HBM.Web.Managers;
using HBM.Web.Models;
using HBM.Web.ViewModels;
using Microsoft.AspNet.Identity;
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
                
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public async Task<ActionResult> Show(int id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return HttpNotFound("User not found");
            UserShowViewModel model = new UserShowViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.UserRole.Key,
                About = user.About,
                Avatar = user.Avatar?.Path,
                Rating = user.UserStats.Rating,
                Articles = user.UserStats.ArticlesPosted,
                Comments = user.UserStats.CommentsWritten,
                Banned = user.UserStats.TimesBanned
            };
            return View(model);
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
                    if (!user.HasPermission(PermissionKey.LogIn))
                        return HttpNotFound("You are not allowed to login on the site. You may be unauthorized");

                    ClaimsIdentity claim = new ClaimsIdentity("ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    claim.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String));
                    claim.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName, ClaimValueTypes.String));
                    claim.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "OWIN Provider", ClaimValueTypes.String));
                    if (user.UserRole != null)
                        claim.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole.Key, ClaimValueTypes.String));

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
                {
                    var user = await UserManager.FindAsync(model.Username, model.Password);
                    var stats = new UserStats()
                    {
                        User = user
                    };
                    user.UserStats = stats;
                    await UserManager.UserDbContext.SaveChangesAsync();
                    return View("Login");
                }
                if (result.HasFlag(UserCreationResult.UserEmailDuplicate))
                    ModelState.AddModelError("Email", "User with such an email is already exists");
                if (result.HasFlag(UserCreationResult.UsernameDuplicate))
                    ModelState.AddModelError("Username", "User with such a name is already exists");
                if (result.HasFlag(UserCreationResult.UnknownError))
                    ModelState.AddModelError("", "Invalid register attempt");
            }
            return View(model);
        }
        
        public async Task<bool> HasPermission(PermissionKey key)
        {
            var user = await GetUser(User.Identity?.GetUserId());
            return user != null && user.HasPermission(key);
        }
        public async Task<ApplicationUser> GetUser(string id) => id != null ? await UserManager.FindByIdAsync(id) : null;
    }    
}