using Backend.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Backend.Controllers
{
    [CustomAuthorize(Roles = "SuperAdmin")]
    public class SuperAdminController : BaseController
    {
        ApiResult apiResult = new ApiResult();
        // GET: Users
        public ActionResult Index(int? page, int? size, string searchText)
        {
            ListUsersViewModel model = new ListUsersViewModel();
            try
            {
                if (!page.HasValue)
                {
                    page = 1;
                }
                if (!size.HasValue)
                {
                    size = 100;
                }
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    int totalUsers = db.Users.Count();
                    model.Pager = new Pager(totalItems: totalUsers, currentPage: page.Value, pageSize: size.Value);
                    model.UsersViewModels = new List<UserViewModel>();
                    var query = db.Users.Where(x => x.IsActive && x.Roles.Any(r => r.Role.SuperClass == "SuperAdmin")).Select(user => new
                    {
                        UserId = user.Id,
                        user.Name,
                        Username = user.UserName,
                        user.UserCode,
                        user.Email,
                        user.PhoneNumber,
                        RoleNames = (from userRole in user.Roles
                                     join role in db.Roles on userRole.RoleId
                                     equals role.Id
                                     select role.Name).ToList(),
                       
                    });
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query = query.Where(w => w.UserCode == searchText);
                    }
                    query = query.OrderBy(o => o.UserId).Skip((page.Value - 1) * size.Value).Take(size.Value);
                    model.UsersViewModels = query.Select(u => new UserViewModel()
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Username = u.Username,
                        UserCode = u.UserCode,
                        Email = u.Email,
                        Mobile = u.PhoneNumber,
                        //  ProfileDetails = u.ProfileDetails,
                        UserRoles = db.Roles.Where(r => u.RoleNames.Contains(r.Name)).Select(r => r.Name).ToList(),
                        Role = db.Roles.Where(r => u.RoleNames.Contains(r.Name)).Select(r => r.Name).FirstOrDefault()
                    }).OrderBy(o => o.Role).ToList();
                }
            }
            catch (Exception ex)
            {
                if (Helpers.WriteExceptionsToEventLog)
                { Helpers.WriteToErrorLog(ex, "Index()"); }
                else
                { throw ex; }
            }
            return View(model);
            //return View();
        }


        // GET: User/Details/5
        public ActionResult Details(string uID)
        {
            UserProfile user = new UserProfile();
            if (!string.IsNullOrEmpty(uID))
            {
              //  user = AppModel.GetUser(uID);
            }
            return View(user);
        }
        // GET: User/Create
        public ActionResult Create()
        {
            ViewData["ROLEselectListItems"] = AppModel.ListOfRoles("SuperAdmin");
            //ViewData["DEPTselectListItems"] = AppModel.DeptSelectListItemsByDeptId();
            var user = new UserProfile();
            return View(user);
        }
        // POST: User/Create
        [HttpPost]
        public JsonResult CreateUser(UserProfileEncrypt user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));
                    ApplicationRoleManager roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));
                    ApplicationUser appUser = new ApplicationUser();


                    var Name = AesOperation.DecryptStringAES(user.Name);
                    var Password = AesOperation.DecryptStringAES(user.Password);
                    var Username = AesOperation.DecryptStringAES(user.Username);
                    var UserCode = AesOperation.DecryptStringAES(user.UserCode);
                    var Email = AesOperation.DecryptStringAES(user.Email);
                    var Mobile = AesOperation.DecryptStringAES(user.Mobile);
                    //var Department = AesOperation.DecryptStringAES(user.Department);
                    var Role = AesOperation.DecryptStringAES(user.Role);



                    appUser.Name = Name;
                    appUser.UserName = Username;
                    appUser.UserCode = UserCode;
                    appUser.Email = Email;
                    appUser.PhoneNumber = Mobile;
                    //appUser.CreatedBy = HttpContext.User.Identity.GetUserId();
                    appUser.CreatedBy = UserInfo.UserId;
                    appUser.CreatedOn = DateTime.Now;
                    appUser.IsActive = true;
                    var result = userManager.Create(appUser, Password);
                    if (result.Succeeded)
                    {
                        //Add Role
                        userManager.AddToRole(appUser.Id,Role);
                        context.SaveChanges();
                        apiResult.IsSuccessful = true;
                        apiResult.Message = "User Account has been created.";
                    }
                    else
                    {
                        string errors = string.Join("", result.Errors);
                        apiResult.IsSuccessful = false;
                        apiResult.Message = "Failed to create User Account. Errors : " + errors;
                    }
                }
                catch (Exception ex)
                {
                    if (Helpers.WriteExceptionsToEventLog)
                    { Helpers.WriteToErrorLog(ex, string.Format("CreateUser({0},{1})", user.Username, user.UserCode)); }
                    else
                    { throw ex; }
                    apiResult.IsSuccessful = false;
                    apiResult.Message = "Failed to create User Account.";
                }
            }
            return Json(apiResult);
        }
        // GET: User/Edit/5
        [HttpGet]
        public ActionResult Edit(string uID)
        {
            var User = new UserProfile();
            try
            {
                if (string.IsNullOrEmpty(uID))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
              //  User = AppModel.GetUser(uID);
                ViewData["ROLEselectListItems"] = AppModel.ListOfRoles("SuperAdmin");
                ViewData["VALIDUSERselectListItems"] = AppModel.VALIDUSERselectListItems();
                //ViewData["DEPTselectListItems"] = AppModel.DeptSelectListItemsByDeptId();
            }
            catch (Exception ex)
            {
                if (Helpers.WriteExceptionsToEventLog)
                { Helpers.WriteToErrorLog(ex, string.Format("Edit({0})", uID)); }
                else
                { throw ex; }
            }
            return View(User);
        }
        [HttpPost]
        public JsonResult EditUser(UserProfileEncrypt user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));
                    ApplicationRoleManager roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));
                    
                    var Name = AesOperation.DecryptStringAES(user.Name);
                    //var Password = AesOperation.DecryptStringAES(user.Password);
                    var Username = AesOperation.DecryptStringAES(user.Username);
                    var UserCode = AesOperation.DecryptStringAES(user.UserCode);
                    var Email = AesOperation.DecryptStringAES(user.Email);
                    var Mobile = AesOperation.DecryptStringAES(user.Mobile);
                    bool IsActive =Convert.ToBoolean(AesOperation.DecryptStringAES(user.IsActive));
                    //var Department = AesOperation.DecryptStringAES(user.Department);
                    var Role = AesOperation.DecryptStringAES(user.Role);

                    ApplicationUser appUser = userManager.FindById(user.UserId);
                    appUser.Name = Name;
                    appUser.UserName = Username;
                    appUser.UserCode = UserCode;
                    if (!string.IsNullOrEmpty(Mobile))
                    {
                        appUser.PhoneNumber = Mobile;
                    }
                    appUser.Email = Email;
                    appUser.UpdatedBy = HttpContext.User.Identity.GetUserId();
                    appUser.UpdatedOn = DateTime.Now;
                    appUser.IsActive = IsActive;
                    List<string> eUserRoles = userManager.GetRoles(user.UserId).ToList();
                    //string eRole = userManager.GetRoles(user.UserId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Email))
                    {
                        var result = userManager.Update(appUser);
                        if (result.Succeeded)
                        {
                            if (eUserRoles != null)
                            {
                                foreach (var r in eUserRoles)
                                {
                                    userManager.RemoveFromRole(appUser.Id, r);
                                }
                            }
                            //for add roles....
                            userManager.AddToRole(appUser.Id,Role);
                            apiResult.IsSuccessful = true;
                            apiResult.Message = "User Account has been updated.";
                        }
                        else
                        {
                            string errors = string.Join("", result.Errors);
                            apiResult.IsSuccessful = false;
                            apiResult.Message = "Failed to modify User Account. Errors : " + errors;
                        }
                    }
                    else
                    {
                        apiResult.IsSuccessful = false;
                        apiResult.Message = "Failed to modify User Account. Email Address Not Found.";
                    }
                }
                catch (Exception ex)
                {
                    if (Helpers.WriteExceptionsToEventLog)
                    { Helpers.WriteToErrorLog(ex, "Edit(id, UserProfile)"); }
                    else
                    { throw ex; }
                    apiResult.IsSuccessful = false;
                    apiResult.Message = "Failed to modify User Account.";
                }
            }
            return Json(apiResult);
        }
        // GET: User/Delete/5
        public ActionResult Delete(string uID)
        {
            if (string.IsNullOrEmpty(uID))
            {
                return RedirectToAction("Index");
            }
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));
                ApplicationUser user = userManager.FindById(uID);
                if (user != null)
                {
                    user.IsActive = false;
                    IdentityResult result = userManager.Update(user);
                    if (result.Succeeded)
                    {
                        ViewBag.Notify = "$.notify({message: 'User Account has been Deleted.'},{type: 'info'});";
                    }
                    else
                    {
                        string errors = string.Join("", result.Errors);
                        ViewBag.Notify = "$.notify({message: '" + errors + "'},{type: 'danger'});";
                    }
                }
            }
            catch (Exception ex)
            {
                if (Helpers.WriteExceptionsToEventLog)
                { Helpers.WriteToErrorLog(ex, string.Format("Delete(id) - {0}", uID)); }
                else
                { throw ex; }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ResetPassword(string userId, string password)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));
                var provider = new DpapiDataProtectionProvider(Helpers.ApplicationName);
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("ASP.NET Identity"));
                var u = userManager.FindById(userId);
                string email = u.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    string role = userManager.GetRoles(u.Id).FirstOrDefault();
                    string token = userManager.GeneratePasswordResetToken(userId);
                    var Password = AesOperation.DecryptStringAES(password);
                    IdentityResult result = userManager.ResetPassword(userId, token, Password);
                    if (result.Succeeded)
                    {
                        apiResult.Message = "Password has been modified";
                        apiResult.IsSuccessful = true;
                    }
                    else
                    {
                        apiResult.IsSuccessful = false;
                        apiResult.Message = string.Join("", result.Errors);
                    }
                }
                else
                {
                    apiResult.IsSuccessful = false;
                    apiResult.Message = "Unable to reset Password due to not avilable Email Address!!!";
                }
            }
            catch (Exception ex)
            {
                if (Helpers.WriteExceptionsToEventLog)
                { Helpers.WriteToErrorLog(ex, "ResetPassword(userId,password)"); }
                else
                { throw ex; }
            }
            return Json(apiResult);
        }

        [HttpPost]
        public JsonResult GeneratePassword()
        {
            int lengthOfPassword = 6;
            string passkey = "abcdefghijklmnozABCDEFGHIJKLMNOZ1234567890";
            StringBuilder strB = new StringBuilder(100);
            Random random = new Random();
            while (0 < lengthOfPassword--)
            {
                strB.Append(passkey[random.Next(passkey.Length)]);
            }
            return Json(new { Result = "success", Password = strB.ToString() });
        }
    }
}
