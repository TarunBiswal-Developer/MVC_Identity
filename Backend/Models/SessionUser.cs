using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend.Models
{
    public class SessionUser
    {
        public UserProfile UserProfile { get; set; }
        public SessionUser()
        {
            UserProfile _userInfo = Get();
            if (string.IsNullOrEmpty(_userInfo.UserId) || string.IsNullOrEmpty(_userInfo.Name) || string.IsNullOrEmpty(_userInfo.Username) || string.IsNullOrEmpty(_userInfo.Role) ||
                string.IsNullOrEmpty(_userInfo.UserCode) || string.IsNullOrEmpty(_userInfo.Email) || _userInfo.UserRoles.Count == 0)
            {
                _userInfo = Set();
            }
            UserProfile = _userInfo;
        }

        private UserProfile Get()
        {
            UserProfile _userInfo = new UserProfile();
            if (HttpContext.Current.Session != null)
            {
                _userInfo.UserId = HttpContext.Current.Session["_ui"] != null ? AesOperation.DecryptString(HttpContext.Current.Session["_ui"].ToString()) : null;
                _userInfo.Name = HttpContext.Current.Session["_n"] != null ? AesOperation.DecryptString(HttpContext.Current.Session["_n"].ToString()) : null;
                _userInfo.Username = HttpContext.Current.Session["_un"] != null ? AesOperation.DecryptString(HttpContext.Current.Session["_un"].ToString()) : null;
                _userInfo.Role = HttpContext.Current.Session["_r"] != null ? AesOperation.DecryptString(HttpContext.Current.Session["_r"].ToString()) : null;
                _userInfo.UserCode = HttpContext.Current.Session["_uc"] != null ? AesOperation.DecryptString(HttpContext.Current.Session["_uc"].ToString()) : null;
                _userInfo.Email = HttpContext.Current.Session["_e"] != null ? AesOperation.DecryptString(HttpContext.Current.Session["_e"].ToString()) : null;
                _userInfo.UserRoles = HttpContext.Current.Session["_ur"] != null ? JsonConvert.DeserializeObject<List<string>>(AesOperation.DecryptString(HttpContext.Current.Session["_ur"].ToString())) : null;
               // _userInfo.UserDepartments = HttpContext.Current.Session["_ud"] != null ? JsonConvert.DeserializeObject<List<DepartmentViewModel>>(AesOperation.DecryptString(HttpContext.Current.Session["_ud"].ToString())) : null;
            }

            return _userInfo;
        }

        private UserProfile Set()
        {
            UserProfile _userInfo = CurrentUser();
            if (_userInfo != null)
            {
                HttpContext.Current.Session["_ui"] = AesOperation.EncryptString(_userInfo.UserId);
                HttpContext.Current.Session["_n"] = AesOperation.EncryptString(_userInfo.Name);
                HttpContext.Current.Session["_un"] = AesOperation.EncryptString(_userInfo.Username);
                HttpContext.Current.Session["_r"] = AesOperation.EncryptString(_userInfo.Role);
                HttpContext.Current.Session["_uc"] = AesOperation.EncryptString(_userInfo.UserCode);
                HttpContext.Current.Session["_e"] = AesOperation.EncryptString(_userInfo.Email);
                HttpContext.Current.Session["_ur"] = AesOperation.EncryptString(JsonConvert.SerializeObject(_userInfo.UserRoles));
               // HttpContext.Current.Session["_ud"] = AesOperation.EncryptString(JsonConvert.SerializeObject(_userInfo.UserDepartments));
            }
            return _userInfo;
        }

        private UserProfile CurrentUser()
        {
            UserProfile _userInfo = new UserProfile();
            string UserName = HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrEmpty(UserName))
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var User = db.Users.Where(m => m.UserName == UserName).Select(user => new
                    {
                        UserId = user.Id,
                        Name = user.Name,
                        Username = user.UserName,
                        UserCode = user.UserCode,
                        Email = user.Email,
                        Role = (from userRole in user.Roles
                                join role in db.Roles on userRole.RoleId
                                equals role.Id
                                select role.Name).ToList()
                    }).FirstOrDefault();
                    if (User != null)
                    {
                        _userInfo.UserId = User.UserId;
                        _userInfo.Name = User.Name;
                        _userInfo.Username = User.Username;
                        if (User.Role.Count > 0)
                        {
                            _userInfo.Role = User.Role.FirstOrDefault();
                            _userInfo.UserRoles = User.Role.ToList();
                        }
                        _userInfo.UserCode = User.UserCode;
                        _userInfo.Email = User.Email;
                        //_userInfo.UserDepartments = db.UserDepartments.Where(d => d.UserId == User.UserId).Join(db.Departments, ud => ud.DeptId, d => d.Id, (ud, d) => new DepartmentViewModel()
                        //{
                        //    DeptId = d.Id,
                        //    DeptCode = d.DeptCode,
                        //    DeptName = d.DeptName,
                        //    DeptShortName = d.DeptShortName
                        //}).ToList();
                    }
                }
            }
            return _userInfo;
        }

    }
}