using Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Backend.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Batch { get; set; }
        public string EmployeeCategotyName { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string UserCode { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }
        public List<string> UserRoles { get; set; }

    }
    public class ListUsersViewModel
    {
        public List<UserViewModel> UsersViewModels { get; set; }
        public Pager Pager { get; set; }
    }




}