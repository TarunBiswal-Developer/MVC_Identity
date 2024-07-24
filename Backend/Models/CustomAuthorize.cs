using System.Web.Mvc;
namespace Backend.Models
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If they are authorized, handle accordingly
            if (AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var Url = new UrlHelper(filterContext.RequestContext);
            var url = Url.Action("Login", "Account");
            filterContext.Result = new RedirectResult(url);
        }
    }
}