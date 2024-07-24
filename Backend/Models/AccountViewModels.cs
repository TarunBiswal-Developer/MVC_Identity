﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Backend.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }
    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }
        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
        public bool RememberMe { get; set; }
    }
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class LoginViewModel
    {
        private string _username;
        private string _password;

        [Required]
        [Display(Name = "Username")]
        public string Username
        {
            get { return _username; }
            set { _username = value.Trim(); }
        }
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password
        {
            get { return _password; }
            set { _password = value.Trim(); }
        }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string Captcha {get;set;}
    }
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Mobile Number.")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
    }
}
