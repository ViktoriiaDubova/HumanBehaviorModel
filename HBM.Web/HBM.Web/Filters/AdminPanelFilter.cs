using System.Web;
using System.Web.Mvc;

namespace HBM.Web.Filters
{
    public class Admin : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.IsAuthenticated && httpContext.User.IsInRole("admin");
        }
    }
}