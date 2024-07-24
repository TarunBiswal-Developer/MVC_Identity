using Backend.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;
namespace Backend.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _AppUserManager = null;
        private UserProfile _userInfo = new SessionUser().UserProfile;
        private string _userId;
        private string _userName;
        public BaseController()
        {
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        protected string UserId
        {
            get
            {
                return _userId ?? _userInfo.UserId;
            }
            set
            {
                _userId = value;
            }
        }
        protected string UserName
        {
            get
            {
                return _userName ?? _userInfo.Username;
            }
            set
            {
                _userName = value;
            }
        }
        protected UserProfile UserInfo
        {
            get
            {
                return _userInfo ?? new SessionUser().UserProfile;
            }
            set
            {
                _userInfo = value;
            }
        }
    }
}