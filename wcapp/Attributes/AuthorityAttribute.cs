using System.Linq;
using System.Web.Mvc;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Attributes
{
    public class AuthorityAttribute : ActionFilterAttribute
    {
        private Authority Authority { get; }
        public AuthorityAttribute(Authority auth)
        {
            Authority = auth;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var user = filterContext.HttpContext.Session.GetSessionUser();
            if (!user.Authorities.Contains(Authority))
            {
                filterContext.HttpContext.Response.Redirect("~/");
            }
        }
    }
}