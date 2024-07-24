using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Backend.Models
{
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }
        public ApplicationUser(string name, string userName, string userCode, string createdBy,int batchId,int employeeCategoryId)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            UserName = userName;
            BatchId = batchId;
            EmployeeCategoryId = employeeCategoryId;
            UserCode = userCode;
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;
            IsActive = true;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("Name", Name));
            return userIdentity;
        }
        [StringLength(50)]
        public string Name { get; set; }
        public string UserCode { get; set; }
        public int? EmployeeCategoryId { get; set; }
        public int? BatchId { get; set; }
        
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole() : base()
        { }
        public ApplicationRole(string name,string displayName,string superClass)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            DisplayName = displayName;
            SuperClass = superClass;
            //WebAccess = webAccess;
            IsActive = true;
        }
        public string DisplayName { get; set; }
        public string SuperClass { get; set; }
        //public bool AppAccess { get; set; }
        //public bool WebAccess { get; set; }
        //public string Section { get; set; }
        public bool IsActive { get; set; }
    }
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>
    {
        public ApplicationRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext() : base("ApplicationDbContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        //dbSet here

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
        }
    }
}