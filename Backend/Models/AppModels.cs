using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Backend.Models
{

    public class AppModel
    {
        public static List<SelectListItem> ROLEselectListItems(bool Islogin = false)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var dbContext = new ApplicationDbContext())
            {
                ApplicationRoleManager roleManager = new ApplicationRoleManager(new ApplicationRoleStore(dbContext));
                if (Islogin)
                {
                    items = roleManager.Roles.Where(r => r.IsActive && r.Name != "Developer").OrderBy(o => o.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                }
                else
                {
                    items = roleManager.Roles.Where(r => r.IsActive && r.Name != "Developer").OrderBy(o => o.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                }
            }
            items.Insert(0, new SelectListItem { Text = "- Select Role -", Value = "" });
            return items;
        }

        
        public static List<SelectListItem> ListOfRoles(string superClass)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var dbContext = new ApplicationDbContext())
            {
                ApplicationRoleManager roleManager = new ApplicationRoleManager(new ApplicationRoleStore(dbContext));
                if (superClass == "NA")
                {
                    items = roleManager.Roles.Where(r => r.IsActive).OrderBy(o => o.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                }
                else
                {
                    items = roleManager.Roles.Where(r => r.IsActive && r.SuperClass == superClass).OrderBy(o => o.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                }
            }
            items.Insert(0, new SelectListItem { Text = "- Select Role -", Value = "" });
            return items;
        }






        public static SelectList SelectRoleWiseUser(string role)
        {
            string Text = "- Select " + role + " -";
            string Selected = "";
            List<SelectListItem> items = new List<SelectListItem>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                items = db.Users.Where(u => u.Roles.Any(r => r.Role.Name.ToLower() == role.ToLower()
                && u.IsActive))
                    .Select(s => new SelectListItem()
                    { Value = s.Id.ToString(), Text = s.Name })
                    .OrderBy(o => o.Text).ToList();

            }
            items.Insert(0, new SelectListItem { Text = Text, Value = "" });
            return new SelectList(items, "Value", "Text", Selected);
        }
        //start




   
     



        public static SelectList GetRolesList(string Selected = "")
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                items = db.Roles
                 .Where(r => r.Name == "Student" || r.Name == "Faculty")
                 .Select(r => new SelectListItem
                 {
                     Value = r.Id,
                     Text = r.Name
                 }).OrderBy(r => r.Text)
                 .ToList();
                items.Insert(0, new SelectListItem { Value = "ALL", Text = "ALL" });
            }
            return new SelectList(items, "Value", "Text", Selected);
        }

        private static ApplicationRoleManager _roleManager;
        private static ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }
        public static List<SelectListItem> ROLEselectListItems(string Role)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var dbContext = new ApplicationDbContext())
            {
                if (Role == "Admin")
                {
                    items = RoleManager.Roles.Where(r => r.IsActive).OrderBy(o => o.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                }
                else
                {
                    items = RoleManager.Roles.Where(r => r.IsActive).OrderBy(o => o.Name).Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                }
            }
            items.Insert(0, new SelectListItem { Text = "- Select Role -", Value = "" });
            return items;
        }
        public static List<SelectListItem> UserSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (var dbContext = new ApplicationDbContext())
            {
                items = dbContext.Users.Where(r => r.IsActive == true && r.UserName != "IIITSA").OrderBy(o => o.UserName).Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() }).ToList();
            }
            items.Insert(0, new SelectListItem { Text = "- Select UserName -", Value = "" });
            return items;
        }
        public static List<SelectListItem> VALIDUSERselectListItems()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "- Select -", Value = "" });
            items.Add(new SelectListItem { Text = "Yes", Value = "true" });
            items.Add(new SelectListItem { Text = "No", Value = "false" });
            return items;
        }
        public static SelectList SelectListYesNo(string Selected = "")
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "- Select -", Value = "" });
            items.Add(new SelectListItem { Text = "Yes", Value = "Yes" });
            items.Add(new SelectListItem { Text = "No", Value = "No" });
            return new SelectList(items, "Value", "Text", Selected);
        }

        public static string formatDate(DateTime? dt)
            => dt == null ? "n/a" : ((DateTime)dt).ToString("dd/MM/yyyy");


    }
    public class ActiveUsers
    {
        public static List<ActiveUser> Get()
        {
            List<ActiveUser> loggedinUserList = null;
            if (HttpContext.Current.Application["ActiveUsers"] == null)
            {
                loggedinUserList = new List<ActiveUser>();
            }
            else
            {
                loggedinUserList = HttpContext.Current.Application["ActiveUsers"] as List<ActiveUser>;
            }
            return loggedinUserList;
        }

        public static void Set(List<ActiveUser> ActiveUsers)
        {
            HttpContext.Current.Application["ActiveUsers"] = ActiveUsers;
        }

        public static void Remove(string UserName)
        {
            List<ActiveUser> loggedinUserList = Get();
            if (loggedinUserList.Count > 0)
            {
                ActiveUser activeUser = loggedinUserList.Where(w => w.UserName == UserName).FirstOrDefault();
                if (activeUser != null)
                {
                    loggedinUserList.Remove(activeUser);
                    HttpContext.Current.Application["ActiveUsers"] = loggedinUserList;
                }
            }
        }

        public static void Clear()
        {
            HttpContext.Current.Application["ActiveUsers"] = null;
        }
    }
    public class ActiveUser
    {
        public string UserName { get; set; }
        public DateTime LoggedOn { get; set; }
    }
    public class ApiResult
    {
        public bool IsSuccessful { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
    public class ApiResult<T>
    {
        public bool IsSuccessful { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}