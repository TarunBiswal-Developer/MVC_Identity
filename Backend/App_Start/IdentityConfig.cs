using Backend.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
//using TemplateParser;

namespace Backend
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return ConfigSendGridAsync(message);
        }
        private async Task ConfigSendGridAsync(IdentityMessage message)
        {
            string apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            string ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
            string ApplicationEmail = ConfigurationManager.AppSettings["ApplicationEmail"];
            SendGridClient client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();
            msg.AddTo(message.Destination);
            msg.From = new EmailAddress(ApplicationEmail, ApplicationName);
            msg.Subject = message.Subject;
            msg.PlainTextContent = message.Body;
            msg.HtmlContent = message.Body;
            var response = await client.SendEmailAsync(msg);
        }
    }
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser, string>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser, string>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser, string>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, string>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
    /// <summary>
    /// The role manager used by the application to store roles and their connections to users
    /// </summary>
    public class ApplicationRoleManager : RoleManager<ApplicationRole, string>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            ///It is based on the same context as the ApplicationUserManager
            var appRoleManager = new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
            return appRoleManager;
        }
    }
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            SignInStatus signInStatus;
            if (UserManager != null)
            {
                /// changed to use email address instead of username
                Task<ApplicationUser> userAwaiter = UserManager.FindByNameAsync(userName);
                ApplicationUser tUser = await userAwaiter;
                if (tUser != null && tUser.IsActive)
                {
                    Task<bool> cultureAwaiter1 = UserManager.IsLockedOutAsync(tUser.Id);
                    if (!await cultureAwaiter1)
                    {
                        Task<bool> cultureAwaiter2 = UserManager.CheckPasswordAsync(tUser, password);
                        if (!await cultureAwaiter2)
                        {
                            if (shouldLockout)
                            {
                                Task<IdentityResult> cultureAwaiter3 = UserManager.AccessFailedAsync(tUser.Id);
                                await cultureAwaiter3;
                                Task<bool> cultureAwaiter4 = UserManager.IsLockedOutAsync(tUser.Id);
                                if (await cultureAwaiter4)
                                {
                                    signInStatus = SignInStatus.LockedOut;
                                    return signInStatus;
                                }
                            }
                            signInStatus = SignInStatus.Failure;
                        }
                        else
                        {
                            Task<IdentityResult> cultureAwaiter5 = UserManager.ResetAccessFailedCountAsync(tUser.Id);
                            await cultureAwaiter5;
                            Task<SignInStatus> cultureAwaiter6 = SignInOrTwoFactor(tUser, isPersistent);
                            signInStatus = await cultureAwaiter6;
                        }
                    }
                    else
                    {
                        signInStatus = SignInStatus.LockedOut;
                    }
                }
                else
                {
                    signInStatus = SignInStatus.Failure;
                }
            }
            else
            {
                signInStatus = SignInStatus.Failure;
            }
            return signInStatus;
        }
        private async Task<SignInStatus> SignInOrTwoFactor(ApplicationUser user, bool isPersistent)
        {
            SignInStatus signInStatus;
            string str = Convert.ToString(user.Id);
            Task<bool> cultureAwaiter = UserManager.GetTwoFactorEnabledAsync(user.Id);
            if (await cultureAwaiter)
            {
                Task<IList<string>> providerAwaiter = UserManager.GetValidTwoFactorProvidersAsync(user.Id);
                IList<string> listProviders = await providerAwaiter;
                if (listProviders.Count > 0)
                {
                    Task<bool> cultureAwaiter2 = AuthenticationManagerExtensions.TwoFactorBrowserRememberedAsync(AuthenticationManager, str);
                    if (!await cultureAwaiter2)
                    {
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity("TwoFactorCookie");
                        claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", str));
                        AuthenticationManager.SignIn(new ClaimsIdentity[] { claimsIdentity });
                        signInStatus = SignInStatus.RequiresVerification;
                        return signInStatus;
                    }
                }
            }
            Task cultureAwaiter3 = SignInAsync(user, isPersistent, false);
            await cultureAwaiter3;
            signInStatus = SignInStatus.Success;
            return signInStatus;
        }
    }
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
             InitializeIdentityForEF();
            base.Seed(context);
        }
        public static void InitializeIdentityForEF()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUserManager userManager = new ApplicationUserManager(new ApplicationUserStore(context));
            ApplicationRoleManager roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));
            if (!roleManager.RoleExists("SuperAdmin"))
            {
                roleManager.Create(new ApplicationRole("SuperAdmin", "SuperAdmin", "SuperAdmin"));
            }
            if (userManager.FindByName("SuperAdmin") == null)
            {
                var User = new ApplicationUser
                {
                    Name = "BackSA",
                    UserCode = "SuperAdmin",
                    UserName = "SAdmin",
                    Email = "sa@gmail.com",
                    IsActive = true
                };
                //string Password = Helpers.GenerateRandomPassword(6);
                string Password = "SA@123";
                var result = userManager.Create(User, Password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(User.Id, "SuperAdmin");
                }
            }
            //// Create More Roles
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new ApplicationRole("Admin", "Admin", "SuperAdmin"));
            }
            // Create More Roles
            if (!roleManager.RoleExists("User"))
            {
                roleManager.Create(new ApplicationRole("User", "User", "SuperAdmin"));
            }
            //if (!roleManager.RoleExists("AccountAdmin"))
            //{
            //    roleManager.Create(new ApplicationRole("AccountAdmin", "AccountAdmin", "SuperAdmin"));
            //}
            //if (!roleManager.RoleExists("LibraryAdmin"))
            //{
            //    roleManager.Create(new ApplicationRole("LibraryAdmin", "LibraryAdmin", "SuperAdmin"));
            //}
            //if (!roleManager.RoleExists("InventoryAdmin"))
            //{
            //    roleManager.Create(new ApplicationRole("InventoryAdmin", "InventoryAdmin", "SuperAdmin"));
            //}
            //if (!roleManager.RoleExists("Student"))
            //{
            //    roleManager.Create(new ApplicationRole("Student", "Student", "AcademyAdmin"));
            //}
            //if (!roleManager.RoleExists("Employee"))
            //{
            //    roleManager.Create(new ApplicationRole("Employee", "Employee", "HrAdmin"));
            //}
            //if (!roleManager.RoleExists("Faculty"))
            //{
            //    roleManager.Create(new ApplicationRole("Faculty", "Faculty", "HrAdmin"));
            //}
        }
    }
}
