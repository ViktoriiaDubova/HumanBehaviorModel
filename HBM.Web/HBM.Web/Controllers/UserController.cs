using System.Web.Mvc;
using HBM.Web.Contexts;

namespace HBM.Web.Controllers
{
    public class UserController : Controller
    {
        private UserDbContext db = new UserDbContext();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose();
        }
    }
}