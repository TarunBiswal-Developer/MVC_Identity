using Backend.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace Backend.Controllers
{
    //[CustomAuthorize(Roles = "SuperAdmin")]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        ApplicationDbContext DbContext;
        public AccountController()
        {
            DbContext = new ApplicationDbContext();
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToActionByRole(returnUrl);
            }
            if (Helpers.ApplicationEnvironment == "D")
            {
                ViewData["ROLEselectListItems"] = AppModel.ROLEselectListItems(true);
                ViewData["USERselectListItems"] = new List<SelectListItem>();
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult GenerateCaptcha ()
        {
            var rand = new Random();
            string captchaCode = GenerateRandomCode(rand, 4);
            Session ["CaptchaCode"] = captchaCode;

            using (var bitmap = new Bitmap(150, 50))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                for (int i = 0; i < 10; i++)
                {
                    int x1 = rand.Next(bitmap.Width);
                    int y1 = rand.Next(bitmap.Height);
                    int x2 = rand.Next(bitmap.Width);
                    int y2 = rand.Next(bitmap.Height);
                    g.DrawLine(new Pen(Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256))), x1, y1, x2, y2);
                }

                var fonts = new [] { "Arial", "Georgia", "Comic Sans MS", "Times New Roman" };
                var colors = new [] { Brushes.Black, Brushes.Red, Brushes.Green, Brushes.Blue };
                var fontSizes = new [] { 18, 20, 22, 24 };

                for (int i = 0; i < captchaCode.Length; i++)
                {
                    var font = new Font(fonts [rand.Next(fonts.Length)], fontSizes [rand.Next(fontSizes.Length)]);
                    var color = colors [rand.Next(colors.Length)];
                    var rotationAngle = rand.Next(-30, 30);
                    var charX = 20 + (i * 30);

                    g.TranslateTransform(charX, 25);
                    g.RotateTransform(rotationAngle);
                    g.DrawString(captchaCode [i].ToString(), font, color, new PointF(0, 0));
                    g.RotateTransform(-rotationAngle);
                    g.TranslateTransform(-charX, -25);
                }

                // Add random dots for noise
                for (int i = 0; i < 100; i++)
                {
                    int x = rand.Next(bitmap.Width);
                    int y = rand.Next(bitmap.Height);
                    bitmap.SetPixel(x, y, Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)));
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }

        private string GenerateRandomCode ( Random rand, int length )
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s [rand.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl, string captchaCode)
        {
            if (ModelState.IsValid)
            {
                if (Session ["CaptchaCode"] == null || captchaCode != Session ["CaptchaCode"].ToString())
                {
                    TempData ["SweetAlert"] = $"error| Login Failed ?|Invalid Captcha!!!|OK|center|custom-container-class";
                    return View(model);
                }
            }

            //var loggedinUser = UserManager.Find(model.Username, model.Password);
            List<ActiveUser> loggedinUserList = ActiveUsers.Get();
            if (loggedinUserList.Where(l => l.UserName == model.Username).Count() > 0)
            {
                TempData["SweetAlert"] = $"error| Login Failed ?|Login Blocked!!! User already logged-in. |OK|true|5000|center|custom-container-class";
                //ViewBag.Notify = "$.notify({message: 'Login Blocked!!! User already logged-in.'},{type: 'danger'});";
                return View(model);
            }
            else
            {
                var result = SignInManager.PasswordSignIn(model.Username, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        //loggedinUserList.Add(new ActiveUser() { UserName = model.Username.ToUpper(), LoggedOn = DateTime.Now });
                        //ActiveUsers.Set(loggedinUserList);
                        return RedirectToAction("Login", "Account", new { returnUrl });
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                    case SignInStatus.Failure:
                        TempData["SweetAlert"] = $"error| Login Failed ?|Login Failed!!! Please, Try Again. If the problem persists, contact administrator. |OK|true|5000|center|custom-container-class";
                        return View(model);
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        TempData["SweetAlert"] = $"error| Login Failed ?|Invalid Login Attempt.Please, Try Again. |OK|true|5000|center|custom-container-class";
                        return View(model);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAsUser(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.Username);
                if (user != null)
                {
                    SignInManager.SignIn(user, true, true);
                    ViewBag.ReturnUrl = returnUrl;
                    return RedirectToAction("Login", "Account", new { returnUrl });
                }
            }
            else
            {
                ViewData["ROLEselectListItems"] = AppModel.ROLEselectListItems(true);
            }
            return View("Login", model);
        }

        // POST: /Account/LogOff
        [CustomAuthorize]
        public ActionResult Logout()
        {
            ActiveUsers.Remove(UserInfo.Username.ToUpper());
            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect(FormsAuthentication.LoginUrl);
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToActionByRole(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            string actionName = "Index";
            string controllerName = "Dashboard";
            string userRole = UserInfo.UserRoles.FirstOrDefault();
            switch (userRole)
            {
                case "SuperAdmin":
                //case "Developer":
                    controllerName = "SuperAdmin";
                    actionName = "Create";
                    break;
                case "Admin":
                    controllerName = "Admin";
                    actionName = "Dashboard";
                    break;
                case "User":
                    controllerName = "User";
                    actionName = "Dashboard";
                    break;
                default:
                    break;
            }

            return RedirectToAction(actionName, controllerName);
        }

        [HttpPost]
        public JsonResult GetRoleList(string role = "ALL")
        {
            var result1 = JsonConvert.SerializeObject(AppModel.ROLEselectListItems(true));
            return Json(new { result1 }, JsonRequestBehavior.AllowGet);
        }

    }
}